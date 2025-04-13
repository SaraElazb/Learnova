using BusinessLogicLayer.DTOs.RoleDto;
using BusinessLogicLayer.Services.RoleServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.ActionRequests.RoleActionRequest;
using PresentationLayer.VMs.RoleVMs;

namespace PresentationLayer.Controllers
{
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var roles = await _roleService.GetAllRolesAsync();
            IEnumerable<RoleVm>? roleVms = roles.Select(r => new RoleVm() { RoleName = r.RoleName }).ToList();
            return View(roleVms);
         
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateRoleActionRequest request)
        {
            if (ModelState.IsValid)
            {
                var (succeeded, errors) = await _roleService.CreateRoleAsync(request.RoleName);
                if (succeeded)
                    return RedirectToAction("Index");

                foreach (var error in errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View();
        }
    }

}
