﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.DTOs
{
    public class TestSectionWithQuestionsDTO
    {
        public string TestSectionID { get; set; }
        public string? Context { get; set; }
        public TestFormatType TestSectionType { get; set; }
        public decimal Score { get; set; }
        public List<QuestionDetailDTO> Questions { get; set; }
    }

}
