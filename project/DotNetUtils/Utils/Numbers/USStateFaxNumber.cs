using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DotNetUtils
{
    public static class USStateFaxNumber
    {
        public static IDictionary<string, string> StateFaxNumbers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"Alabama","3342066020"},
            {"Alaska","9077873197"},
            {"Arizona","8882820502"},
            {"Arkansas","8002593562"},
            {"California","9163194400"},
            {"Colorado","3032972595"},
            {"Connecticut","8008161108"},
            {"Delaware","8554810047"},
            {"Florida","8888544762"},
            {"Georgia","8885410521"},
            {"Hawaii","8086927001"},
            {"Idaho","2083327411"},
            {"Illinois","2175571947"},
            {"Indiana","3176123036"},
            {"Iowa","8007595881"},
            {"Kansas","8882197798"},
            {"Kentucky","8008170099"},
            {"Louisiana","8554810047"},
            {"Maine","8004379611"},
            {"Maryland","4102816004"},
            {"Massachusetts","6173763262"},
            {"Michigan","8773181659"},
            {"Minnesota","8006924473"},
            {"Mississippi","8009378668"},
            {"Missouri","5735268079"},
            {"Montana","8882721990"},
            {"Nebraska","8668082007"},
            {"Nevada","7756846379"},
            {"New Hampshire","6032240825"},
            {"New Jersey","5183201080"},
            {"New Mexico","8888781614"},
            {"New York","5183201080"},
            {"North Carolina","8662577005"},
            {"North Dakota","7013285497"},
            {"Ohio","8888721611"},
            {"Oklahoma","18003173786"},
            {"Oregon","8778777415"},
            {"Pennsylvania","8667484473"},
            {"Rhode Island","8884306907"},
            {"South Carolina","8038989100"},
            {"South Dakota","8888358659"},
            {"Tennessee","18775054761"},
            {"Texas","18007325015"},
            {"Utah","8015264391"},
            {"Vermont","8028284286"},
            {"Virginia","8006882680"},
            {"Washington","8007820624"},
            {"West Virginia","8776254675"},
            {"Wisconsin","8002778075"},
            {"Wyoming","8009219651"},
        };

        public static string GetStateFaxNumber(string state, bool appendCode = false)
        {
            return StateFaxNumbers.ContainsKey(state)
                ? string.Format(CultureInfo.InvariantCulture, appendCode ? "1{0}" : "{0}", StateFaxNumbers[state])
                : null;
        }
    }
}
