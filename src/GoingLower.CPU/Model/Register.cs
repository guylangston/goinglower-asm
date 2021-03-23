using System;
using System.Collections.Generic;
using GoingLower.Core.Primitives;

namespace GoingLower.CPU.Model
{
    public class Register : Prop<ulong>
    {
        public Register(string id, string name) : base(PropHelper.DefaultCompareULong, 0, null)
        {
            Id   = id;
            Name = name;
        }

        public string  Id            { get; set; }
        public string  Name          { get; set; }
        public string  Description   { get; set; }
        public string? LastUsedAs    { get; set; }
        public bool    IsExtendedReg { get; set; }
        public string? TagValue      { get; set; }
        
        public IReadOnlyList<string> IdAlt { get; set; }  // TODO: Refactor to use RegisterMode
        
        public string ValueHex => Value.ToString("X").PadLeft(64 / 8 * 2, '0');

        public int LastUsedAsSize
        {
            get
            {
                if (LastUsedAs == null) return 64;
                if (LastUsedAs.StartsWith("e")) return 32;

                return 64;

            }
        }

        

        public IEnumerable<string> AllIds()
        {
            yield return Id;
            foreach (var ii in IdAlt)
            {
                yield return ii;
            }
        }
        
        

        public override string ToString() => $"{Id}/{Name} = {Value:X}";

        public bool Match(string rId)
        {
            if (string.Equals(Id, rId, StringComparison.InvariantCultureIgnoreCase)) return true;

            if (IdAlt != null)
            {
                foreach (var a in IdAlt)
                {
                    if (string.Equals(a, rId, StringComparison.InvariantCultureIgnoreCase)) return true;    
                }    
            }
            

            return false;
        }

        public RegisterDelta ToDelta()
        {
            return new RegisterDelta()
            {
                Register    = Id,
                ValueParsed = Value,
                ValueRaw    = ValueHex
            };
        }
    }
}