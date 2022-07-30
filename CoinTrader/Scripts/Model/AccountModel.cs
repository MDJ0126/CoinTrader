using Network;
using System.Collections.Generic;

public class AccountModel
{
    public List<AccountRes> Accounts = new List<AccountRes>();

    /// <summary>
    /// 잔고 세팅
    /// </summary>
    /// <param name="res"></param>
    public void SetAccount(List<AccountRes> res)
    {
        this.Accounts.Clear();
        this.Accounts.AddRange(res);
    }
}