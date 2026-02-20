using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Bater_Ponto.Data;
using Bater_Ponto.Models;
using Bater_Ponto.Identity;
using System;
using System.Linq;

namespace Bater_Ponto.Controllers
{
    [Authorize]
    public class PontoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PontoController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);

            var registros = _context.RegistrosPonto
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.Data)
                .ToList();

            return View(registros);
        }

        [HttpPost]
        public IActionResult Registrar()
        {
            var userId = _userManager.GetUserId(User);

            DateTime agora = DateTime.Now;
            DateTime hoje = agora.Date;

            var registroHoje = _context.RegistrosPonto
                .FirstOrDefault(r => r.Data == hoje && r.UserId == userId);

            if (registroHoje == null)
            {
                registroHoje = new RegistroPonto
                {
                    Data = hoje,
                    EntradaManha = agora,
                    UserId = userId
                };

                _context.RegistrosPonto.Add(registroHoje);
            }
            else if (registroHoje.SaidaAlmoco == default)
            {
                registroHoje.SaidaAlmoco = agora;
            }
            else if (registroHoje.VoltaAlmoco == default)
            {
                registroHoje.VoltaAlmoco = agora;
            }
            else if (registroHoje.SaidaFinal == default)
            {
                registroHoje.SaidaFinal = agora;
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Excluir(int id)
        {
            var userId = _userManager.GetUserId(User);

            var registro = _context.RegistrosPonto
                .FirstOrDefault(r => r.Id == id && r.UserId == userId);

            if (registro != null)
            {
                _context.RegistrosPonto.Remove(registro);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}