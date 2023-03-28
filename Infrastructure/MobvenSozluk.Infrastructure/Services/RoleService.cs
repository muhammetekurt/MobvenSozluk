﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MobvenSozluk.Domain.Concrete.Entities;
using MobvenSozluk.Infrastructure.Exceptions;
using MobvenSozluk.Repository.DTOs.CustomQueryDTOs;
using MobvenSozluk.Repository.DTOs.EntityDTOs;
using MobvenSozluk.Repository.DTOs.ResponseDTOs;
using MobvenSozluk.Repository.Repositories;
using MobvenSozluk.Repository.Services;
using MobvenSozluk.Repository.UnitOfWorks;
using System.Data;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MobvenSozluk.Infrastructure.Services
{
    public class RoleService : Service<Role,RoleDto>, IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPagingService<Role> _pagingService;
        private readonly ISortingService<Role> _sortingService;
        private readonly IFilteringService<Role> _filteringService;
        private readonly ISearchingService<Role> _searchingService;
        private readonly IErrorMessageService _errorMessageService;

        public RoleService(IGenericRepository<Role> repository, IUnitOfWork unitOfWork, IRoleRepository roleRepository, IMapper mapper, IPagingService<Role> pagingService, ISortingService<Role> sortingService, IFilteringService<Role> filteringService, RoleManager<Role> roleManager, UserManager<User> userManager, ISearchingService<Role> searchingService, IErrorMessageService errorMessageService) : base(repository, unitOfWork, sortingService, pagingService, mapper, filteringService, searchingService, errorMessageService)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;

            _roleManager = roleManager;
            _userManager = userManager;
            _searchingService = searchingService;
            _errorMessageService = errorMessageService;
        }

        public async Task<CustomResponseDto<RoleDto>> CreateAsync(AddRoleDto roleDto)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleDto.Name);

            if (roleExists)
            {
                throw new ConflictException(_errorMessageService.RoleAlreadyExist);
            }

            var role = new Role
            {
                Name = roleDto.Name
            };

            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                throw new BadRequestException(_errorMessageService.BadRequestDescription);
            }

            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");

            foreach(var user in adminUsers)
            {
                await _userManager.AddToRoleAsync(user, role.Name);
            }

            var createdRole = new RoleDto
            {
                Id = role.Id,
                Name = role.Name
            };

            return CustomResponseDto<RoleDto>.Success(200, createdRole);
          
        }

       
        public async Task<CustomResponseDto<RoleDto>> EditAsync(RoleDto roleDto)
        {
            var databaseRole = await _roleManager.FindByIdAsync(roleDto.Id.ToString());

            if (databaseRole == null)
            {
                throw new NotFoundException(_errorMessageService.NotFoundMessage<Role>());
            }

            databaseRole.Name = roleDto.Name;

            await _roleManager.UpdateAsync(databaseRole);

            return CustomResponseDto<RoleDto>.Success(200, roleDto);

        }

        public async Task<CustomResponseDto<RoleByIdWithUsersDto>> GetRoleByIdWithUsers(int roleId)
        {
           
            var role = await _roleRepository.GetRoleByIdWithUsers(roleId);
     
            if(role== null)
            {
                throw new NotFoundException(_errorMessageService.NotFoundMessage<Role>());
            }

            var roleDto = _mapper.Map<RoleByIdWithUsersDto>(role);

            return CustomResponseDto<RoleByIdWithUsersDto>.Success(200, roleDto);
        }
    }
}
