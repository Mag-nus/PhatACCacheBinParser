﻿using System.IO;

using PhatACCacheBinParser.Common;

namespace PhatACCacheBinParser.Seg6_LandBlockExtendedData
{
	class Weenie
	{
		public uint WCID;

		public Position Position = new Position();

		// Phat only uses a 1 byte ID, but the value in the bin is 4 bytes. I don't know why the Phat JSONs only output 1 byte for the id.
		public uint ID;
		//public byte IDb;

		public bool Unpack(BinaryReader binaryReader)
		{
			WCID = binaryReader.ReadUInt32();

			Position.Unpack(binaryReader);

			ID = binaryReader.ReadUInt32();
			//IDb = (byte)ID;

			return true;
		}
	}
}
