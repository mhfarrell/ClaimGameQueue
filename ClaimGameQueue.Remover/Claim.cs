using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClaimGameQueue.Remover
{
    public class Claim
    {
        public Claim(Guid userId, Guid regionId)
        {
            UserId = userId;
            RegionId = regionId;
            Claims = 1;
        }
        public Guid UserId { get; set; }
        public Guid RegionId { get; set; }
        public int Claims { get; set; }
    }
}
