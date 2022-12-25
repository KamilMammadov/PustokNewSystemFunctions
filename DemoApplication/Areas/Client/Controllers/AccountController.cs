using DemoApplication.Areas.Client.ViewModels.Account;
using DemoApplication.Areas.Client.ViewModels.Authentication;
using DemoApplication.Database;
using DemoApplication.Database.Models;
using DemoApplication.Services.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoApplication.Areas.Client.Controllers
{
    [Area("client")]
    [Route("account")]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IUserService _userService;

        public AccountController(DataContext dataContext, IUserService userService)
        {
            _dataContext = dataContext;
            _userService = userService;
        }

        [HttpGet("dashboard", Name = "client-account-dashboard")]
        public IActionResult Dashboard()
        {
            var user = _userService.CurrentUser;
            var user2 = _userService.CurrentUser;

            return View();
        }

        [HttpGet("orders", Name = "client-account-orders")]
        public IActionResult Orders()
        {
            var user = _userService.CurrentUser;
            var user2 = _userService.CurrentUser;

            return View();
        }


       
            [HttpGet("details", Name = "client-account-details")]
            public async Task<IActionResult> Details()
            {

                var currentUser = _userService.CurrentUser;

                var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == currentUser.Id);

                var model = new UserViewModel
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                };

                return View(model);
            }


        [HttpPost("details", Name = "client-account-details")]
        public async Task<IActionResult> Details(UserViewModel model)
        {
            var currentuser = _userService.CurrentUser;
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == currentuser.Id);

            user.FirstName=model.FirstName;
            user.LastName = model.LastName;
            await _dataContext.SaveChangesAsync();

            return RedirectToRoute("client-account-details");

        }




        [HttpPost("detailsPassword",Name = "client-account-details-password")]
        public async Task<IActionResult> DetailsPassword(PasswordUpdateViewModel newModel)
        {
            var user = _userService.CurrentUser;
            var model = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);

            if (model == null) return NotFound();

            bool verified = BCrypt.Net.BCrypt.Verify(newModel.lastPassword, model.Password);
            if (verified)
            {
                var newHashPassword= BCrypt.Net.BCrypt.HashPassword(newModel.Password);
                model.Password = newHashPassword;
                _dataContext.SaveChanges();
                
            }



            return RedirectToRoute("client-account-details");

        }


        [HttpGet("address", Name = "client-account-address")]
        public async Task<IActionResult> Address()
        {

            var currentUser = _userService.CurrentUser;

            var address = await _dataContext.UserAddresses.FirstOrDefaultAsync(a => a.UserId == currentUser.Id);

            if (address is null) return RedirectToRoute("client-account-address-edit",new EditAddressViewModel());
            

            var model = new AddressListViewModel
            {
                User=$"{address.User.FirstName} {address.User.LastName}",
                Address=address.Address,
                PhoneNumber=address.PhoneNumber,
            };

            return View(model);
        }
        [HttpGet("editaddress", Name = "client-account-address-edit")]
        public async Task<IActionResult> EditAddress()
        {

            var currentUser = _userService.CurrentUser;

            var address = await _dataContext.UserAddresses.FirstOrDefaultAsync(a => a.UserId == currentUser.Id);

            if (address is null)
            {
                return View(new EditAddressViewModel());
            }


            var model = new EditAddressViewModel
            {
                Address = address!.Address,
                PhoneNumber = address.PhoneNumber,
            };

            return View(model);
        }

        [HttpPost("editaddress", Name = "client-account-address-edit")]
        public async Task<IActionResult> EditAddress(EditAddressViewModel model)
        {
            var currentuser = _userService.CurrentUser;
            var address = await _dataContext.UserAddresses.FirstOrDefaultAsync(u => u.UserId == currentuser.Id);

            if (address is null)
            {
                var newAddress = new UserAddress
                {
                    UserId=currentuser.Id,
                    Address=model.Address,
                    PhoneNumber=model.PhoneNumber,
                };
                await _dataContext.UserAddresses.AddAsync(newAddress);
            }
            else
            {
                address.Address = model.Address;
                address.PhoneNumber = model.PhoneNumber;
            }
            
            await _dataContext.SaveChangesAsync();

            return RedirectToRoute("client-account-address");

        }

    }
}
