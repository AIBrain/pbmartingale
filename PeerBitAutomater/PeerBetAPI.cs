using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeerBitAutomater
{
    class PeerBetAPI
    {
        private String _APIKey;
        private String _PBUser;
        private String _PBPassword;
        private Double _balance;
        private Double _Spent_Total;
        private Double _Won_Total;
        private Double _Deposit_Total;
        private Double _Withdrawals_Total;
        private Double _ProfitFromPB;
        private Double _TodayProfitFromPB;

        private Boolean _LoggedIn = false;


        public String APIKey
        {
            get { return _APIKey; }
        }

        public Double Balance
        {
            get { return _balance; }
        }

        public PeerBetAPI(String user, String pass)
        {
            _PBUser = user;
            _PBPassword = pass;
            this.LoginPeerBet();
            this.RefreshPeerBetGetUserInfo();

        }



        /// <summary>
        /// Logins to Peer Bit
        /// </summary>
        /// <returns>Whether successful</returns>
        public Boolean LoginPeerBet()
        {
            Boolean SuccessLogin = false;

            PBWebRequest pbRequest = new PBWebRequest();
            String sLoginResponse = pbRequest.GetPBResponse("method=login&username=" + _PBUser + "&password=" + _PBPassword);
            try
            {
                var results = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(sLoginResponse);
                _APIKey = results.key;

                if (results.success == 1)
                {
                    SuccessLogin = true;
                    _LoggedIn = true;
                }
                else
                {
                    _LoggedIn = false;
                }
            }
            catch (Exception e)
            {
                SuccessLogin = false;
            }

            return (SuccessLogin);
        }

        /// <summary>
        /// Refreshes the Peer Bet user info.
        /// </summary>
        /// <returns></returns>
        public Boolean RefreshPeerBetGetUserInfo()
        {
            Boolean returnOk = false;

            if (_LoggedIn == false)
            {
                this.LoginPeerBet();
            }
            try
            {
                PBWebRequest pbRequest = new PBWebRequest();
                String sLoginResponse = pbRequest.GetPBResponse("method=getuserinfo&key=" + _APIKey);
                var results = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(sLoginResponse);
                if (results.username == "none")
                {
                    this.LoginPeerBet();
                    sLoginResponse = pbRequest.GetPBResponse("method=getuserinfo&key=" + _APIKey);
                    results = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(sLoginResponse);
                }
                _balance = Double.Parse(results.balance.ToString());
                _Spent_Total = Double.Parse(results.spent_total.ToString());
                _Won_Total = Double.Parse(results.won_total.ToString());
                _Deposit_Total = Double.Parse(results.deposits_total.ToString());
                _Withdrawals_Total = Double.Parse(results.withdrawals_total.ToString());
                _ProfitFromPB = Double.Parse(results.profit.ToString());
                _TodayProfitFromPB = Double.Parse(results.profit_today.ToString());
                returnOk = true;
            }
            catch (Exception e)
            {
                returnOk = false;
            }

            return (returnOk);

        }
    }
}
