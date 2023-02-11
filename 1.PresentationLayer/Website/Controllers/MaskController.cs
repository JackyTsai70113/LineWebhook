using System.Collections.Generic;
using BL.Service.Interface;
using Core.Domain.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Website.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaskController : ControllerBase
    {
        private readonly IMaskInstitutionService MaskInstitutionService;

        public MaskController(IMaskInstitutionService maskInstitutionService)
        {
            MaskInstitutionService = maskInstitutionService;
        }

        [HttpGet]
        [Route("count")]
        public ActionResult<int> Count()
        {
            int maskInstitutionCount = MaskInstitutionService.GetMaskInstitutionCount();
            return maskInstitutionCount;
        }

        [HttpGet]
        [Route("list")]
        public ActionResult<List<MaskInstitution>> List()
        {
            string address = "110台灣台北市信義區虎林街132巷37號";
            return MaskInstitutionService.GetMaskInstitutions(address);
        }
    }
}