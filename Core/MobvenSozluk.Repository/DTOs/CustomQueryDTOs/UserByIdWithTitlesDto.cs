﻿using MobvenSozluk.Repository.DTOs.EntityDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobvenSozluk.Repository.DTOs.CustomQueryDTOs
{
    public class UserByIdWithTitlesDto: UserDto
    {
        public List<TitleDto> Titles { get; set; }
    }
}
