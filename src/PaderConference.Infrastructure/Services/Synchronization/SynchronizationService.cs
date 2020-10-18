﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonPatchGenerator;
using Microsoft.AspNetCore.SignalR;
using PaderConference.Core.Domain.Entities;
using PaderConference.Infrastructure.Serialization;
using PaderConference.Infrastructure.Sockets;

namespace PaderConference.Infrastructure.Services.Synchronization
{
    public class SynchronizationService : ConferenceService, ISynchronizationManager
    {
        private readonly IHubClients _clients;
        private readonly string _conferenceId;
        private readonly IConnectionMapping _connectionMapping;

        private readonly Guid _id = Guid.NewGuid();

        private readonly ConcurrentDictionary<string, ISynchronizedObject> _registeredObjects =
            new ConcurrentDictionary<string, ISynchronizedObject>();

        public SynchronizationService(IHubClients clients, string conferenceId,
            IConnectionMapping connectionMapping)
        {
            _clients = clients;
            _conferenceId = conferenceId;
            _connectionMapping = connectionMapping;
        }

        public ISynchronizedObject<T> Register<T>(string name, T initialValue) where T : notnull
        {
            var obj = new SynchronizedObject<T>(initialValue,
                (oldValue, newValue) => UpdateObject(oldValue, newValue, name));

            if (!_registeredObjects.TryAdd(name, obj))
                throw new InvalidOperationException("An object with the same name was already registered.");

            return obj;
        }

        public override async ValueTask InitializeParticipant(Participant participant)
        {
            var connectionId = _connectionMapping.ConnectionsR[participant];
            var state = GetState();

            await _clients.Client(connectionId)
                .SendAsync(CoreHubMessages.Response.OnSynchronizeObjectState, state);
        }

        public IReadOnlyDictionary<string, object> GetState()
        {
            return _registeredObjects.ToDictionary(x => x.Key, x => x.Value.GetCurrent());
        }

        private async ValueTask UpdateObject<T>(T oldValue, T newValue, string name) where T : notnull
        {
            var patch = JsonPatchFactory.CreatePatch(oldValue, newValue);
            if (!patch.Operations.Any()) return;

            await _clients.Group(_conferenceId).SendAsync(
                CoreHubMessages.Response.OnSynchronizedObjectUpdated,
                new SynchronizedObjectUpdatedDto(name,
                    patch.Operations.Select(x => new SerializableJsonPatchOperation(x)).ToList()));
        }
    }
}