﻿using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Network
{
    /// <summary>
    /// 전체 계좌 조회
    /// </summary>
    public class AccountRes : iResponse
    {
        /// <summary>
        /// 화폐를 의미하는 영문 대문자 코드
        /// </summary>
        public string currency;
        /// <summary>
        /// 주문가능 금액/수량
        /// </summary>
        public double balance;
        /// <summary>
        /// 주문 중 묶여있는 금액/수량
        /// </summary>
        public double locked;
        /// <summary>
        /// 매수평균가
        /// </summary>
        public double avg_buy_price;
        /// <summary>
        /// 매수평균가 수정 여부
        /// </summary>
        public bool avg_buy_price_modified;
        /// <summary>
        /// 평단가 기준 화폐
        /// </summary>
        public string unit_currency;
    }

    public class HandlerAccount : ProtocolHandler
    {
        private List<AccountRes> res = null;

        public HandlerAccount()
        {
            this.URI = new Uri(ProtocolManager.BASE_URL + "accounts");
            this.Method = Method.Get;
        }

        public async Task<List<AccountRes>> Request()
        {
            RestRequest request = new RestRequest(URI, Method);
            request.AddHeader("Authorization", ProtocolManager.GetAuthToken());
            request.AddHeader("Accept", "application/json");
            await base.RequestProcess(request);
            return res;
        }

        protected override void Response(RestRequest request, RestResponse response)
        {
            if (response.IsSuccessful)
            {
                res = JsonParser<AccountRes>(response.Content);
                ModelCenter.Account.SetAccount(res);
            }
            else
            {
                if (response.ErrorMessage != null)
                    Logger.Error(response.ErrorMessage);
            }
        }
    }
}
