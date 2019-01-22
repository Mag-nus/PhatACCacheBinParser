﻿using System;
using System.Collections.Generic;
using System.IO;

using PhatACCacheBinParser.Common;

namespace PhatACCacheBinParser.Seg9_WeenieDefaults
{
    class Weenie : IUnpackable
    {
        // _cache_bin_parse_9_1
        public uint FileVersion;

        public string Description;

        public uint Timestamp;

        public int WeenieType;

        public Dictionary<int, int> IntValues;

        public Dictionary<int, long> LongValues;

        public Dictionary<int, bool> BoolValues;

        public Dictionary<int, double> DoubleValues;

        public Dictionary<int, string> StringValues;

        public Dictionary<int, uint> DIDValues;

        public Dictionary<int, Position> PositionValues;

        public Dictionary<int, uint> IIDValues;

        // _cache_bin_parse_9_8
        public uint WCID;

        // _cache_bin_parse_9_9
        public Attributes Attributes;

        public Dictionary<int, Skill> Skills;

        public Dictionary<int, BodyPart> BodyParts;

        public Dictionary<int, float> SpellCastingProbability;

        public List<int> EventFilters;

        public Dictionary<int, List<Emote>> Emotes;

        public List<Item> CreateList;

        public PagesData PagesData;

        public List<Generator> Generators;


        public Palette Palette;

        public List<TextureMap> TextureMaps;

        public List<AnimPart> AnimParts;

        public bool IsAutoGenerated;


        // Unix timestamp is seconds past epoch
        //static DateTime UnixTimeStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public bool Unpack(BinaryReader reader)
        {
            int count;

            // _cache_bin_parse_9_1

            // I believe this is a start of record identifier
            FileVersion = reader.ReadUInt32(); // 02000000, is it always this value?

            Description = Util.ReadString(reader, true);

            Timestamp = reader.ReadUInt32();
            //TimestampAsString = UnixTimeStart.AddSeconds(Timestamp).ToString();

            var basicFlags = reader.ReadInt32();
            WeenieType = reader.ReadInt32();

            if ((basicFlags & 0x01) == 0x01)
            {
                IntValues = new Dictionary<int, int>();

                count = reader.ReadUInt16();
                reader.ReadUInt16(); // discard

                for (int i = 0; i < count; i++)
                    IntValues.Add(reader.ReadInt32(), reader.ReadInt32());
            }

            if ((basicFlags & 0x80) == 0x80)
            {
                LongValues = new Dictionary<int, long>();

                count = reader.ReadUInt16();
                reader.ReadUInt16(); // discard

                for (int i = 0; i < count; i++)
                    LongValues.Add(reader.ReadInt32(), reader.ReadInt64());
            }

            if ((basicFlags & 0x02) == 0x02)
            {
                BoolValues = new Dictionary<int, bool>();

                count = reader.ReadUInt16();
                reader.ReadUInt16(); // discard

                for (int i = 0; i < count; i++)
                    BoolValues.Add(reader.ReadInt32(), Convert.ToBoolean(reader.ReadInt32()));
            }

            if ((basicFlags & 0x04) == 0x04)
            {
                DoubleValues = new Dictionary<int, double>();

                count = reader.ReadUInt16();
                reader.ReadUInt16(); // discard

                for (int i = 0; i < count; i++)
                    DoubleValues.Add(reader.ReadInt32(), reader.ReadDouble());
            }

            if ((basicFlags & 0x08) == 0x08)
            {
                StringValues = new Dictionary<int, string>();

                count = reader.ReadUInt16();
                reader.ReadUInt16(); // discard

                for (int i = 0; i < count; i++)
                    StringValues.Add(reader.ReadInt32(), Util.ReadString(reader, true));
            }

            if ((basicFlags & 0x10) == 0x10)
            {
                DIDValues = new Dictionary<int, uint>();

                count = reader.ReadUInt16();
                reader.ReadUInt16(); // discard

                for (int i = 0; i < count; i++)
                    DIDValues.Add(reader.ReadInt32(), reader.ReadUInt32());
            }

            if ((basicFlags & 0x20) == 0x20)
            {
                PositionValues = new Dictionary<int, Position>();

                count = reader.ReadUInt16();
                reader.ReadUInt16(); // discard

                for (int i = 0; i < count; i++)
                {
                    var key = reader.ReadInt32();

                    var value = new Position();

                    value.Unpack(reader);

                    PositionValues.Add(key, value);
                }
            }

            if ((basicFlags & 0x40) == 0x40)
            {
                IIDValues = new Dictionary<int, uint>();

                count = reader.ReadUInt16();
                reader.ReadUInt16(); // discard

                for (int i = 0; i < count; i++)
                    IIDValues.Add(reader.ReadInt32(), reader.ReadUInt32());
            }

            // _cache_bin_parse_9_8
            var extendedFlags = reader.ReadInt32(); // 0000003D, 0000003F, 0000001F, 0000000F, ....., 0000012F
            WCID = reader.ReadUInt32();

            // _cache_bin_parse_9_9
            if (extendedFlags != 0)
            {
                if ((extendedFlags & 0x01) == 0x01)
                {
                    var attributeMask = reader.ReadInt32(); // 000001FF
                    if (attributeMask != 0x1FF)
                        throw new NotImplementedException();

                    Attributes = new Attributes();
                    Attributes.Unpack(reader);
                }


                if ((extendedFlags & 0x02) == 0x02)
                {
                    Skills = new Dictionary<int, Skill>();

                    count = reader.ReadUInt16();
                    reader.ReadUInt16(); // discard

                    for (int i = 0; i < count; i++)
                    {
                        var key = reader.ReadInt32();

                        var value = new Skill();

                        value.Unpack(reader);

                        Skills.Add(key, value);
                    }
                }


                if ((extendedFlags & 0x04) == 0x04)
                {
                    BodyParts = new Dictionary<int, BodyPart>();

                    count = reader.ReadUInt16();
                    reader.ReadUInt16(); // discard

                    for (int i = 0; i < count; i++)
                    {
                        var key = reader.ReadInt32();

                        // ?? Probably number of body.body_part_table.value objects, which should always be 1
                        var unknown = reader.ReadInt32();

                        if (unknown != 1)
                            throw new NotImplementedException();

                        var value = new BodyPart();
                        value.Unpack(reader);

                        BodyParts.Add(key, value);
                    }
                }


                if ((extendedFlags & 0x100) == 0x100)
                {
                    SpellCastingProbability = new Dictionary<int, float>();

                    count = reader.ReadUInt16();
                    reader.ReadUInt16(); // discard

                    for (int i = 0; i < count; i++)
                        SpellCastingProbability.Add(reader.ReadInt32(), reader.ReadSingle());
                }


                if ((extendedFlags & 0x08) == 0x08)
                {
                    EventFilters = new List<int>();

                    count = reader.ReadUInt16();
                    reader.ReadUInt16(); // discard

                    for (int i = 0; i < count; i++)
                        EventFilters.Add(reader.ReadInt32());
                }


                if ((extendedFlags & 0x10) == 0x10)
                {
                    Emotes = new Dictionary<int, List<Emote>>();

                    count = reader.ReadUInt16();
                    reader.ReadUInt16(); // discard

                    for (int i = 0; i < count; i++)
                    {
                        var key = reader.ReadInt32();

                        var value = new List<Emote>();

                        var count2 = reader.ReadUInt16();
                        reader.ReadUInt16(); // discard

                        for (int j = 0; j < count2; j++)
                        {
                            var emote = new Emote();

                            emote.Unpack(reader);

                            value.Add(emote);
                        }

                        Emotes.Add(key, value);
                    }
                }


                if ((extendedFlags & 0x20) == 0x20)
                {
                    CreateList = new List<Item>();

                    count = reader.ReadUInt16();
                    reader.ReadUInt16(); // discard

                    for (int i = 0; i < count; i++)
                    {
                        var value = new Item();

                        value.Unpack(reader);

                        CreateList.Add(value);
                    }
                }


                // I don't know if the order is correct for this data segment. I just assumed based on its mask.
                if ((extendedFlags & 0x40) == 0x40)
                {
                    PagesData = new PagesData();
                    PagesData.Unpack(reader);
                }

                // I don't know if the order is correct for this data segment. I just assumed based on its mask.
                if ((extendedFlags & 0x80) == 0x80)
                {
                    Generators = new List<Generator>();

                    count = reader.ReadUInt16();
                    reader.ReadUInt16(); // discard

                    for (int i = 0; i < count; i++)
                    {
                        var generator = new Generator();
                        generator.Unpack(reader);
                        Generators.Add(generator);
                    }
                }
            }


            // _cache_bin_parse_9_19 - ObjDesc

            // I believe this is a record segment end/start identifier
            var unknown_footer_1 = reader.ReadByte();    // 0x11
            if (unknown_footer_1 != 0x11)
                throw new Exception();

            var numberOfSubpallets = reader.ReadByte();
            var numberOfTextureMaps = reader.ReadByte();
            var numberOfAnimParts = reader.ReadByte();

            if (numberOfSubpallets > 0)
            {
                Palette = new Palette();

                Palette.Unpack(reader, numberOfSubpallets);
            }

            if (numberOfTextureMaps > 0)
            {
                TextureMaps = new List<TextureMap>();

                for (int i = 0; i < numberOfTextureMaps; i++)
                {
                    var item = new TextureMap();
                    item.Unpack(reader);
                    TextureMaps.Add(item);
                }
            }

            if (numberOfAnimParts > 0)
            {
                AnimParts = new List<AnimPart>();

                for (int i = 0; i < numberOfAnimParts; i++)
                {
                    var item = new AnimPart();
                    item.Unpack(reader);
                    AnimParts.Add(item);
                }
            }

            // Make sure our position is a multiple of 4
            if (reader.BaseStream.Position % 4 != 0)
                reader.BaseStream.Position += 4 - (reader.BaseStream.Position % 4);


            // _cache_bin_parse_9_1
            IsAutoGenerated = (reader.ReadByte() == 1); // Always 0x01

            return true;
        }
    }
}
