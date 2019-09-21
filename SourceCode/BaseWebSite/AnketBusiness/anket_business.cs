using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurveyBusiness
{
    public class anket_business 
    {
        SurveyDataAccess.anket_dataaccess _dataAccess = new SurveyDataAccess.anket_dataaccess();
        
        public void SurveyExcelDosyaEkle(Guid id, byte[] imageSize)
        {
            _dataAccess.SurveyExcelDosyaEkle(id, imageSize);
        }

        public void SurveyLogoEkle(Guid anket_uid, byte[] imageSize)
        {
            _dataAccess.SurveyLogoEkle(anket_uid, imageSize);
        }

        public void SurveyLogoyuSil(Guid anket_uid)
        {
            _dataAccess.SurveyLogoyuSil(anket_uid);
        }
    }
}
