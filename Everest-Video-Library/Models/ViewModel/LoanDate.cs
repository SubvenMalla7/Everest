using Everest_Video_Library.Models.VideoLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everest_Video_Library.Models.ViewModel
{
    public class LoanDate
    {
        public string AlbumName { get; set; }
        public List<Loan> Loans { get; set; }
        public List<Loan> LoanDates { get; set; }
        public void MakeLoanDate(string AlbumName,List<Loan> loans)
        {
            var loan  = loans.GroupBy(X => X.LoanDate);
            foreach(Loan i in loan)
            {

            }

        }
    }
}