﻿using Chat_Server.NET.IO;
using System.Net.Sockets;

namespace Chat_Server
{
	class Client
	{
		public string Username { get; set; }
		public Guid UID { get; set; }
		public TcpClient ClientSocket { get; set; }

		PacketReader _packetReader;

		public Client(TcpClient client)
		{
			ClientSocket = client;
			UID = Guid.NewGuid();
			_packetReader = new PacketReader(ClientSocket.GetStream());

			var opcode = _packetReader.ReadByte();
			Username = _packetReader.ReadMessage();

			Console.WriteLine($"[{DateTime.Now}]: Client has connected with the username: {Username}");

			Task.Run(() => Process());
		}

		void Process()
		{
			while (true)
			{
				try
				{
					var opcode = _packetReader.ReadByte();
					switch (opcode)
					{
						case 5:
							var msg = _packetReader.ReadMessage();
							Console.WriteLine($"[{DateTime.Now}] {Username}: {msg}");
							Program.BroadcastMessage($"[{DateTime.Now}] {Username}: {msg}");
							break;
						default:
							break;
					}
				}
				catch (Exception)
				{
					Console.WriteLine($"[{UID.ToString()}]: Disconnected!");
					Program.BroadcastDisconnect(UID.ToString());
					ClientSocket.Close();
					break;
				}
			}
		}
	}
}
