using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Domain.Enums
{
    public enum EnumBloodGroup
    {
        APos = 1,
        ANeg = 2,
        BPos = 3,
        BNeg = 4,
        ABPos = 5,
        ABNeg = 6,
        ZeroPos = 7,
        ZeroNeg = 8,
    }

    public class StrBloodGroup
    {
        public static string StrEnumBloodGroup(EnumBloodGroup enumBloodGroup)
        {
            switch (enumBloodGroup)
            {
                case EnumBloodGroup.APos:
                    return "A+";
                case EnumBloodGroup.ANeg:
                    return "A-";
                case EnumBloodGroup.BPos:
                    return "B+";
                case EnumBloodGroup.BNeg:
                    return "B-";
                case EnumBloodGroup.ABPos:
                    return "AB+";
                case EnumBloodGroup.ABNeg:
                    return "AB-";
                case EnumBloodGroup.ZeroPos:
                    return "0+";
                case EnumBloodGroup.ZeroNeg:
                    return "0-";
                default:
                    return "0-";
            }
        }
    }
}
