using AutoMapper;
using HumanResource.API.DTOs;
using HumanResource.API.DTOs.Common;
using HumanResource.API.Models;
using HumanResource.API.Repositories.Interfaces;
using HumanResource.API.Services.Interfaces;
using HumanResource.API.Exceptions;

namespace HumanResource.API.Services.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        private readonly IMapper _mapper;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            IMapper mapper)
        {
            _employeeRepository = employeeRepository;

            _mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
        {
            var employees =
                await _employeeRepository.GetAllAsync();

            var employeeDtos =
                _mapper.Map<IEnumerable<EmployeeDto>>(employees);

            return employeeDtos;
        }

        public async Task<EmployeeDto> GetByIdAsync(int id)
        {
            var employee =
                await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
            {
                throw new NotFoundException(
                    $"Employee with Id {id} not found");
            }

            var employeeDto =
                _mapper.Map<EmployeeDto>(employee);

            return employeeDto;
        }

        public async Task<EmployeeDto> AddAsync(
            EmployeeDto employeeDto)
        {
            var employee =
                _mapper.Map<Employee>(employeeDto);

            var addedEmployee =
                await _employeeRepository.AddAsync(employee);

            var addedEmployeeDto =
                _mapper.Map<EmployeeDto>(addedEmployee);

            return addedEmployeeDto;
        }

        public async Task<EmployeeDto> UpdateAsync(
            EmployeeDto employeeDto)
        {
            var employee =
                _mapper.Map<Employee>(employeeDto);

            var updatedEmployee =
                await _employeeRepository.UpdateAsync(employee);

            var updatedEmployeeDto =
                _mapper.Map<EmployeeDto>(updatedEmployee);

            return updatedEmployeeDto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var isDeleted =
                await _employeeRepository.DeleteAsync(id);

            if (!isDeleted)
            {
                throw new NotFoundException(
                    $"Employee with Id {id} not found");
            }

            return true;
        }

        public async Task<IEnumerable<EmployeeDto>>
            GetByDepartmentAsync(int departmentId)
        {
            var employees =
                await _employeeRepository
                    .GetByDepartmentAsync(departmentId);

            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public async Task<IEnumerable<EmployeeDto>>
            GetByManagerAsync(int managerId)
        {
            var employees =
                await _employeeRepository
                    .GetByManagerAsync(managerId);

            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public async Task<IEnumerable<EmployeeDto>>
            GetByJobAsync(string jobId)
        {
            var employees =
                await _employeeRepository
                    .GetByJobAsync(jobId);

            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public async Task<IEnumerable<EmployeeDto>>
            GetByRoleAsync(int roleId)
        {
            var employees =
                await _employeeRepository
                    .GetByRoleAsync(roleId);

            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public async Task<IEnumerable<EmployeeDto>>
            SearchAsync(string keyword)
        {
            var employees =
                await _employeeRepository.SearchAsync(keyword);

            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public async Task<PaginatedResponseDto<EmployeeDto>>
            GetPaginatedAsync(int pageNumber, int pageSize)
        {
            var result =
                await _employeeRepository
                    .GetPaginatedAsync(pageNumber, pageSize);

            var employeeDtos =
                _mapper.Map<IEnumerable<EmployeeDto>>(
                    result.Employees);

            return new PaginatedResponseDto<EmployeeDto>
            {
                PageNumber = pageNumber,

                PageSize = pageSize,

                TotalRecords = result.TotalRecords,

                TotalPages = (int)Math.Ceiling(
                    (double)result.TotalRecords / pageSize),

                Data = employeeDtos
            };
        }
        public async Task<IEnumerable<MyTeamEmployeeDto>> GetMyTeamAsync(decimal managerId)
        {
            var employees = await _employeeRepository
                .GetMyTeamAsync(managerId);

            return _mapper.Map<IEnumerable<MyTeamEmployeeDto>>(employees);
        }
        public async Task<EmployeeDto>
            GetHighestSalaryEmployeeAsync()
        {
            var employee =
                await _employeeRepository
                    .GetHighestSalaryEmployeeAsync();

            return _mapper.Map<EmployeeDto>(employee);
        }
    }
}