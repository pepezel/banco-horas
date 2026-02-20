using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Bater_Ponto.Data;
using Bater_Ponto.Models;
using System;
using System.Linq;

namespace Bater_Ponto.Controllers
{
    [Authorize]
    public class PontoController : Controller
    {
        private readonly AppDbContext _context;

        public PontoController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var registros = _context.RegistrosPonto
                .OrderByDescending(r => r.Data)
                .ToList();

            return View(registros);
        }

        [HttpPost]
        public IActionResult Registrar()
        {
            DateTime agora = DateTime.Now;
            DateTime hoje = agora.Date;

            var registroHoje = _context.RegistrosPonto
                .FirstOrDefault(r => r.Data == hoje);

            if (registroHoje == null)
            {
                registroHoje = new RegistroPonto
                {
                    Data = hoje,
                    EntradaManha = agora
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
            var registro = _context.RegistrosPonto.Find(id);

            if (registro != null)
            {
                _context.RegistrosPonto.Remove(registro);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}