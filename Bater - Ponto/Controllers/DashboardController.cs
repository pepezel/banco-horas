using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Bater_Ponto.Data;
using Bater_Ponto.Models;
using System;
using System.Linq;

namespace Bater_Ponto.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(DateTime? dataInicio, DateTime? dataFim)
        {
            var registros = _context.RegistrosPonto.AsQueryable();

            if (dataInicio.HasValue)
                registros = registros.Where(r => r.Data >= dataInicio.Value);

            if (dataFim.HasValue)
                registros = registros.Where(r => r.Data <= dataFim.Value);

            var lista = registros
                .OrderByDescending(r => r.Data)
                .ToList();

            ViewBag.DataInicio = dataInicio;
            ViewBag.DataFim = dataFim;

            TimeSpan saldoTotal = TimeSpan.Zero;
            TimeSpan totalTrabalhado = TimeSpan.Zero;

            int diasPositivos = 0;
            int diasNegativos = 0;

            foreach (var r in lista)
            {
                var totalDia = r.CalcularTotalTrabalhado();
                var saldoDia = r.CalcularSaldoDia();

                if (totalDia != null)
                    totalTrabalhado += totalDia.Value;

                if (saldoDia != null)
                {
                    saldoTotal += saldoDia.Value;

                    if (saldoDia.Value > TimeSpan.Zero)
                        diasPositivos++;

                    if (saldoDia.Value < TimeSpan.Zero)
                        diasNegativos++;
                }
            }

            ViewBag.TotalTrabalhado = totalTrabalhado;
            ViewBag.SaldoTotal = saldoTotal;
            ViewBag.DiasPositivos = diasPositivos;
            ViewBag.DiasNegativos = diasNegativos;

            return View(lista);
        }
    }
}