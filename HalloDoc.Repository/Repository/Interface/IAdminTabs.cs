﻿using HalloDoc.Entity.Models.ViewModel;

namespace HalloDoc.Repository.Repository.Interface
{
    public interface IAdminTabs
    {
        public AdminProfile ViewAdminProfile(string UserId);
        public bool EditPassword(string Password, int UserId);
        public bool EditAdministratorInfo(AdminProfile AdminProfile);
        public bool EditBillingInfo(AdminProfile AdminProfile);
        public List<PhysiciansData> PhysicianAll(int region);
        public bool changeNoti(int[] files, int region);
        public bool ContactProviderMail(string Email, string Message);
    }
}
