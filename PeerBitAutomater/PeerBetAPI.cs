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

        public Double TodayProfitFromPB
        {
            get { return _TodayProfitFromPB; }
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

        /// <summary>
        /// Places the order.
        /// </summary>
        /// <param name="OrderID">The order ID.</param>
        /// <param name="NumberofTickets">The number of tickets.</param>
        /// <returns>if successful</returns>
        public Boolean PlaceOrder(String OrderID, int NumberofTickets)
        {
            Boolean returnOk = false;

            if (_LoggedIn == false)
            {
                this.LoginPeerBet();
            }
            try
            {
                PBWebRequest pbRequest = new PBWebRequest();
                String sPlaceOrderResponse = pbRequest.GetPBResponse("method=buytickets&key=" 
                                + _APIKey + "&raffle=" + OrderID + "&tickets=" + NumberofTickets.ToString());
                if (sPlaceOrderResponse.IndexOf("error") > -1)
                {
                    this.LoginPeerBet();
                    sPlaceOrderResponse = pbRequest.GetPBResponse("method=getuserinfo&key=" + _APIKey);
                }

                var results = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(sPlaceOrderResponse);
                if (results.success == "1")
                    returnOk = true;

            }
            catch (Exception e)
            {
                returnOk = false;
            }

            return (returnOk);

        }

        /// <summary>
        /// Checks if won given order.
        /// </summary>
        /// <param name="OrderID">The order ID.</param>
        /// <returns>True if won, False if lost</returns>
        public Boolean CheckIfWonOrder(String OrderID)
        {
            Boolean returnOk = false;

            if (_LoggedIn == false)
            {
                this.LoginPeerBet();
            }
            try
            {
                PBWebRequest pbRequest = new PBWebRequest();
                String sPlaceOrderResponse = pbRequest.GetPBResponse("method=getraffleinfo&key="
                                + _APIKey + "&raffle=" + OrderID);
                if (sPlaceOrderResponse.IndexOf("error") > -1)
                {
                    this.LoginPeerBet();
                    sPlaceOrderResponse = pbRequest.GetPBResponse("method=getraffleinfo&key="
                                + _APIKey + "&raffle=" + OrderID);
                }

                var results = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(sPlaceOrderResponse);
                if (results.my_ticket_list == results.winning_ticket)
                    returnOk = true;
                else
                    returnOk = false;
            }
            catch (Exception e)
            {
                returnOk = false;
            }

            return (returnOk);

        }

        /// <summary>
        /// Finds the lowest instant order that is a 50/50.
        /// </summary>
        /// <param name="OrderID">The order ID.</param>
        /// <param name="TicketPrice">The ticket price.</param>
        /// <returns></returns>
        public Boolean GetLowestInstantHalfChance(ref String OrderID, ref Double TicketPrice)
        {
            Boolean goodReturn = false;

            Double lowestTicketPrice = 99.0;
            String assocOrderId = "";

            PBWebRequest pbRequest = new PBWebRequest();

            String strGetARafflesResponse = pbRequest.GetPBResponse("method=getactiveraffles");

            var rafflesResults = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(strGetARafflesResponse);

            foreach (var raffle in rafflesResults)
            {
                if (raffle.instant == 1 && raffle.tickets_total == 2)
                {
                    if (raffle.ticket_price < lowestTicketPrice)
                    {
                        assocOrderId = raffle.raffle_id;
                        lowestTicketPrice = raffle.ticket_price;
                    }
                }
            }

            OrderID = assocOrderId;
            TicketPrice = lowestTicketPrice;

            return(goodReturn);
        }

        /// <summary>
        /// Gets the closest order to the ticket amount specified.
        /// Searches only instant orders with a double value
        /// </summary>
        /// <param name="ticketAmount">The ticket amount.</param>
        /// <param name="OrderID">The order ID.</param>
        /// <param name="TicketPrice">The ticket price.</param>
        /// <returns></returns>
        public Boolean GetClosestOrderInstantDouble(Double ticketAmount, ref String OrderID, ref Double TicketPrice)
        {
            Boolean goodReturn = false;

            Double lastRemainder = 99.0;
            String assocOrderId = "";
            Double associatedTicketPrice = 0.0;

            PBWebRequest pbRequest = new PBWebRequest();

            String strGetARafflesResponse = pbRequest.GetPBResponse("method=getactiveraffles");

            var rafflesResults = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(strGetARafflesResponse);

            foreach (var raffle in rafflesResults)
            {
                if (raffle.instant == 1 && raffle.tickets_total == 2)
                {
                    Double dblTP = raffle.ticket_price;
                    if (Math.Abs(ticketAmount - dblTP) < lastRemainder)
                    {
                        assocOrderId = raffle.raffle_id;
                        associatedTicketPrice = raffle.ticket_price;
                        lastRemainder = Math.Abs(ticketAmount - dblTP);
                    }
                }
            }

            OrderID = assocOrderId;
            TicketPrice = associatedTicketPrice;

            return (goodReturn);
        }


    }
}
