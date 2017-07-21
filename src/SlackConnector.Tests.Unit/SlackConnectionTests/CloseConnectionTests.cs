﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture.NUnit3;
using SlackConnector.Connections.Sockets;
using SlackConnector.Models;

namespace SlackConnector.Tests.Unit.SlackConnectionTests
{
    internal class CloseConnectionTests
    {
        [Test, AutoMoqData]
        public async Task should_close_websocket_when_websocket_is_connected([Frozen]Mock<IWebSocketClient> webSocket, SlackConnection slackConnection)
        {
            // given
            webSocket
                .Setup(x => x.IsAlive)
                .Returns(true);

            var info = GetDummyConnectionInformation(webSocket);
            await slackConnection.Initialise(info);

            // when
            await slackConnection.Close();

            // then
            webSocket.Verify(x => x.Close(), Times.Once);
        }

        [Test, AutoMoqData]
        public async Task should_not_close_websocket_when_websocket_is_disconnected([Frozen]Mock<IWebSocketClient> webSocket, SlackConnection slackConnection)
        {
            // given
            webSocket
                .Setup(x => x.IsAlive)
                .Returns(false);

            var info = GetDummyConnectionInformation(webSocket);
            await slackConnection.Initialise(info);

            // when
            await slackConnection.Close();

            // then
            webSocket.Verify(x => x.Close(), Times.Never);
        }

        private static ConnectionInformation GetDummyConnectionInformation(Mock<IWebSocketClient> webSocket)
        {
            var info = new ConnectionInformation
            {
                Self = new ContactDetails { Id = "self-id" },
                Team = new ContactDetails { Id = "team-id" },
                Users = new Dictionary<string, SlackUser> { { "userid", new SlackUser() { Name = "userName" } } },
                SlackChatHubs = new Dictionary<string, SlackChatHub> { { "some-hub", new SlackChatHub() } },
                WebSocket = webSocket.Object
            };
            return info;
        }
    }
}