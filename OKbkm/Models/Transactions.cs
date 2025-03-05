namespace OKbkm.Models
{
    public class Transactions
    {
        public int id { get; set; }
        public int AccountNo { get; set; }
        public decimal Total { get; set; }     // bunu kaldırabilirim sanki balance varya 
        public decimal Deposit { get; set; } // para yatırma
        public decimal Withdrawal { get; set; } // para çekme
        public decimal Transfer { get; set; } // fast işlemine yönlendirebilir
        public decimal Balance { get; set; } // hesapbakiyesi
    }
}
