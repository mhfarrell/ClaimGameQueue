using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClaimGameQueue.Remover
{
    public class Claim
    {
        public Claim(string userId, string regionId)
        {
            UserId = userId;
            RegionId = regionId;
            Claims = 1;
        }
        public string UserId { get; set; }
        public string RegionId { get; set; }
        public int Claims { get; set; }
    }
}
