using Stripe;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Models
{
    public class MakePayment
    {
        public static async Task<dynamic> PayAsync(string cardNumber, int month, int year, string cvc, int value)
        {
            try
            {
                StripeConfiguration.ApiKey = "sk_test_51Jl6muIX515m6kcZvnqzyC32yGpP2a3EDRrBxC2LEnJtCX11BUEON8AixcxEambtpMlFpVjQiPbN63fi6NMbQc7N00DxZ945tT";

                var optionstoken = new TokenCreateOptions
                {
                    Card = new TokenCardOptions
                    {
                        Number = cardNumber,
                        ExpMonth = month,
                        ExpYear = year,
                        Cvc = cvc
                    }
                };

                var serviceToken = new TokenService();
                Token stripeToken = await serviceToken.CreateAsync(optionstoken);

                var options = new ChargeCreateOptions
                {
                    Amount = value,
                    Currency = "usd",
                    Description = "test",
                    Source = stripeToken.Id
                };

                var service = new ChargeService();
                Charge charge = await service.CreateAsync(options);

                if (charge.Paid)
                {
                    return "Success";
                } else
                {
                    return "Failed";
                }
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
    }
}
