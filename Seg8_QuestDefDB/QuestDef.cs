﻿using System.IO;

namespace PhatACCacheBinParser.Seg8_QuestDefDB
{
	class QuestDef : IUnpackable
	{
		public string Name;

		public uint MinDelta;
		public int MaxSolves;

		public string Message;

		public bool Unpack(BinaryReader reader)
		{
			Name = Util.ReadString(reader, true);

			MinDelta = reader.ReadUInt32();
			MaxSolves = reader.ReadInt32();

			Message = Util.ReadEncryptedString1(reader, true);

			return true;
		}
	}
}
