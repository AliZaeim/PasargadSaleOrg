using DataLayer.Entities.User;


namespace Core.DTOs.General
{
    public class RolePool_UserInfo
    {
        public User User { get; set; }
        public long DirectSalesValue { get; set; }
        public long InDirectSalesValue { get; set; }
        public int Percent { get; set; }
        public long Value { get; set; }
    }
}
