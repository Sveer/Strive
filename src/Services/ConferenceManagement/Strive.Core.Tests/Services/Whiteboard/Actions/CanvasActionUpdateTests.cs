﻿using Microsoft.AspNetCore.JsonPatch;
using Moq;
using Strive.Core.Services.Whiteboard;
using Strive.Core.Services.Whiteboard.Actions;
using Strive.Core.Services.Whiteboard.CanvasData;
using Xunit;

namespace Strive.Core.Tests.Services.Whiteboard.Actions
{
    public class CanvasActionUpdateTests
    {
        private const string ParticipantId = "123";
        private readonly JsonPatchDocument<CanvasObject> _undoPatch = new();
        private readonly Mock<ICanvasActionUtils> _utils = new();

        public CanvasActionUpdateTests()
        {
            _utils.Setup(x => x.CreatePatch(It.IsAny<CanvasObject>(), It.IsAny<CanvasObject>())).Returns(_undoPatch);
        }

        [Fact]
        public void Execute_ObjectExists_Patch()
        {
            // arrange
            var action = new CanvasActionUpdate(new[]
            {
                new CanvasObjectPatch(new JsonPatchDocument<CanvasObject>().Add(x => x.ScaleY, 2.0), "1"),
            }, ParticipantId);

            var canvas = WhiteboardCanvas.Empty with
            {
                Objects = new[] {new StoredCanvasObject(new CanvasLine {ScaleY = 1}, "1")},
            };

            // act
            var (updatedCanvas, undoAction) = action.Execute(canvas, _utils.Object);

            // assert
            Assert.Equal(new[] {new StoredCanvasObject(new CanvasLine {ScaleY = 2}, "1")}, updatedCanvas.Objects);

            var undoUpdate = Assert.IsType<CanvasActionUpdate>(undoAction);
            Assert.Equal(new[] {new CanvasObjectPatch(_undoPatch, "1")}, undoUpdate.Patches);
        }

        [Fact]
        public void Execute_ObjectDoesNotExist_DontThrow()
        {
            // arrange
            var action = new CanvasActionUpdate(new[]
            {
                new CanvasObjectPatch(new JsonPatchDocument<CanvasObject>().Add(x => x.ScaleY, 2.0), "1"),
            }, ParticipantId);

            var canvas = WhiteboardCanvas.Empty;

            // act
            var (updatedCanvas, undoAction) = action.Execute(canvas, _utils.Object);

            // assert
            Assert.Equal(canvas.Objects, updatedCanvas.Objects);

            var undoUpdate = Assert.IsType<CanvasActionUpdate>(undoAction);
            Assert.Empty(undoUpdate.Patches);
        }
    }
}
