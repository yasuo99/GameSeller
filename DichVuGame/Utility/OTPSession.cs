using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DichVuGame.Utility
{
    public class OTPSession
    {
        public string Email { get; set; }   
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public int OTP { get; set; }    
        public DateTime ExpireTime { get; set; }
        public OTPSession(int otp, DateTime expireTime,string email,string password,bool remember)
        {
            OTP = otp;
            ExpireTime = expireTime;
            Email = email;
            Password = password;
            RememberMe = remember;
        }
    }
}
