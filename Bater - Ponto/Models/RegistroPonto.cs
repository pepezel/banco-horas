using System;

namespace Bater_Ponto.Models
{
    public class RegistroPonto
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }

        public DateTime EntradaManha { get; set; }
        public DateTime SaidaAlmoco { get; set; }
        public DateTime VoltaAlmoco { get; set; }
        public DateTime SaidaFinal { get; set; }

        public TimeSpan? CalcularTotalTrabalhado()
        {
            if (EntradaManha == default ||
                SaidaAlmoco == default ||
                VoltaAlmoco == default ||
                SaidaFinal == default)
            {
                return null;
            }

            TimeSpan manha = SaidaAlmoco - EntradaManha;
            TimeSpan tarde = SaidaFinal - VoltaAlmoco;

            return manha + tarde;
        }

        public TimeSpan? CalcularSaldoDia()
        {
            var total = CalcularTotalTrabalhado();

            if (total == null)
                return null;

            TimeSpan cargaPadrao = TimeSpan.FromHours(8.5);

            return total.Value - cargaPadrao;
        }
    }
}