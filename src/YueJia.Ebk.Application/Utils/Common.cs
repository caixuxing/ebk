using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YueJia.Ebk.Domain.Shared.Const;

namespace YueJia.Ebk.Application.Utils
{
    public class Common
    {

        public static string GetSearchCode(List<long> Ids) {
           return  CompressedEncryptor.Encrypt(System.Text.Json.JsonSerializer.Serialize(Ids), SecretKeyConst.key, SecretKeyConst.iv);
        }


        public static List<long> AnalysisSearchCode(string SearchCode) {
           var searchCodeStr = CompressedEncryptor.Decrypt(SearchCode, SecretKeyConst.key, SecretKeyConst.iv);
           return System.Text.Json.JsonSerializer.Deserialize<List<long>>(searchCodeStr).ToList();
        }

        public static int AdjustmentPriceForHotel(decimal costPrice, AdjustmentPriceTypeEnum AdjustmentPriceType, int AdjustmentPriceValue)
        {
            decimal salePrice = 0;
            switch (AdjustmentPriceType)
            {
                case AdjustmentPriceTypeEnum.FixedValueIncrease:
                    salePrice = AdjustmentPriceValue > 0 ? costPrice + AdjustmentPriceValue : costPrice;
                    break;
                case AdjustmentPriceTypeEnum.PercentageIncrease:
                    salePrice = AdjustmentPriceValue > 0 ? Math.Round((costPrice * (Convert.ToDecimal(AdjustmentPriceValue) / Convert.ToDecimal(100) + 1)), 0) : costPrice;
                    break;
                default:
                    salePrice = costPrice + 20;//如果没有设置加价，则默认(底价+20元)
                    break;
            }
            return Convert.ToInt32(Math.Max(salePrice, costPrice));
        }

    }
}
