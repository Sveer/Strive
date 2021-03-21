﻿using System.Runtime.Serialization;

namespace PaderConference.Core.Services.Media.Dtos
{
    public enum ProducerSource
    {
        Mic,
        Webcam,
        Screen,

        [EnumMember(Value = "loopback-mic")] LoopbackMic,

        [EnumMember(Value = "loopback-webcam")]
        LoopbackWebcam,

        [EnumMember(Value = "loopback-screen")]
        LoopbackScreen,
    }
}
