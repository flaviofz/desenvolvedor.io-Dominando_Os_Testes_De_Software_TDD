using FluentValidation;
using System;

namespace NerdStore.Vendas.Domain
{
    public class VoucherAplicavelValidation : AbstractValidator<Voucher>
    {
        public static string CodigoErroMsg => "Voucher sem código válido.";
        public static string DataValidadeErroMsg => "Este voucher está expirado.";
        public static string AtivoErroMsg => "Este voucher não é mais válido.";
        public static string UtilizadoErroMsg => "Este voucher já foi utilizado.";
        public static string QuantidadeErroMsg => "Este voucher não está mais disponível";
        public static string ValorDescontoErroMsg => "O valor do desconto precisa ser superior a 0";
        public static string PercentualDescontoErroMsg => "O valor da porcentagem de desconto precisa ser superior a 0";

        public VoucherAplicavelValidation()
        {
            RuleFor(c => c.Codigo)
                .NotEmpty()
                .WithMessage(CodigoErroMsg);

            RuleFor(c => c.DataValidade)
                .Must(DataVencimentoSuperiorAtual)
                .WithMessage(DataValidadeErroMsg);

            RuleFor(c => c.Ativo)
                .Equal(true)
                .WithMessage(AtivoErroMsg);

            RuleFor(c => c.Utilizado)
                .Equal(false)
                .WithMessage(UtilizadoErroMsg);

            RuleFor(c => c.Quantidade)
                .GreaterThan(0)
                .WithMessage(QuantidadeErroMsg);

            When(f => f.TipoDescontoVoucher == TipoDescontoVoucher.Valor, () =>
            {
                RuleFor(f => f.ValorDesconto)
                    .NotNull()
                    .WithMessage(ValorDescontoErroMsg)
                    .GreaterThan(0)
                    .WithMessage(ValorDescontoErroMsg);
            });

            When(f => f.TipoDescontoVoucher == TipoDescontoVoucher.Percentual, () =>
            {
                RuleFor(f => f.PercentualDesconto)
                    .NotNull()
                    .WithMessage(PercentualDescontoErroMsg)
                    .GreaterThan(0)
                    .WithMessage(PercentualDescontoErroMsg);
            });
        }

        protected static bool DataVencimentoSuperiorAtual(DateTime dataValidade)
        {
            return dataValidade >= DateTime.Now;
        }
    }
}
