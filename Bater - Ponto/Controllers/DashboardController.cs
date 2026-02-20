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
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index(DateTime? dataInicio, DateTime? dataFim)
        {
            var userId = _userManager.GetUserId(User);

            var registros = _context.RegistrosPonto
                .Where(r => r.UserId == userId)
                .AsQueryable();

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

            // 🎯 META DIÁRIA

            var hoje = DateTime.UtcNow.Date;

            var registroHoje = _context.RegistrosPonto
                .FirstOrDefault(r => r.UserId == userId && r.Data == hoje);

            var metaDiaria = ConfiguracoesSistema.MetaDiaria;

            TimeSpan horasTrabalhadasHoje = TimeSpan.Zero;

            if (registroHoje != null)
            {
                var total = registroHoje.CalcularTotalTrabalhado();
                if (total != null)
                    horasTrabalhadasHoje = total.Value;
            }

            double percentual = 0;

            if (metaDiaria.TotalMinutes > 0)
                percentual = (horasTrabalhadasHoje.TotalMinutes / metaDiaria.TotalMinutes) * 100;

            if (percentual > 100)
                percentual = 100;

            ViewBag.HorasTrabalhadasHoje = horasTrabalhadasHoje;
            ViewBag.PercentualHoje = percentual;
            ViewBag.MetaDiaria = metaDiaria;

            TimeSpan restante = metaDiaria - horasTrabalhadasHoje;
            bool metaConcluida = restante <= TimeSpan.Zero;

            if (restante < TimeSpan.Zero)
                restante = TimeSpan.Zero;

            ViewBag.RestanteHoje = restante;
            ViewBag.MetaConcluida = metaConcluida;

            // 📊 META SEMANAL AUTOMÁTICA (SEGUNDA A SEXTA)

            var inicioSemana = hoje.AddDays(-(int)hoje.DayOfWeek + (int)DayOfWeek.Monday);
            if (inicioSemana > hoje)
                inicioSemana = inicioSemana.AddDays(-7);

            var fimSemana = inicioSemana.AddDays(5);

            var registrosSemana = _context.RegistrosPonto
                .Where(r => r.UserId == userId &&
                            r.Data >= inicioSemana &&
                            r.Data < fimSemana)
                .ToList();

            TimeSpan totalSemana = TimeSpan.Zero;

            foreach (var r in registrosSemana)
            {
                var totalDia = r.CalcularTotalTrabalhado();
                if (totalDia != null)
                    totalSemana += totalDia.Value;
            }

            // calcular quantos dias úteis já passaram
            int diasUteisPassados = 0;
            for (var d = inicioSemana; d <= hoje && d < fimSemana; d = d.AddDays(1))
            {
                if (d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday)
                    diasUteisPassados++;
            }

            var metaSemanaEsperada = TimeSpan.FromTicks(metaDiaria.Ticks * diasUteisPassados);
            var saldoSemana = totalSemana - metaSemanaEsperada;

            ViewBag.TotalSemana = totalSemana;
            ViewBag.MetaSemanaEsperada = metaSemanaEsperada;
            ViewBag.SaldoSemana = saldoSemana;

            return View(lista);
        }
    }
}