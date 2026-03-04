using Business.Abstract;
using Common;
using Common.Enums;
using DataAccess;
using DataAccess.Abstract;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Business.Services
{
    public class HRService : IHRService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public HRService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task HireEmployeeAsync(int positionId, int employeeId, DateOnly hireDate)
        {
            await using var unitOfWork = new UnitOfWork(_contextFactory);
            var position = await unitOfWork.StructureNodes.GetByIdAsync(positionId) ??
                throw new BusinessValidationException("Позиция не найдена");
            if (position.NodeType != NodeType.Position)
                throw new BusinessValidationException("Сотрудника можно назначить только на должность");
            var employee = await unitOfWork.Employees.GetByIdAsync(employeeId) ??
                throw new BusinessValidationException("Сотрудник не найден");

            position.EmployeeId = employeeId;
            position.HireDate = hireDate;

            unitOfWork.StructureNodes.Update(position);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DismissEmployeeAsync(int positionId)
        {
            await using var unitOfWork = new UnitOfWork(_contextFactory);
            var position = await unitOfWork.StructureNodes.GetByIdAsync(positionId) ??
                throw new BusinessValidationException("Позиция не найдена");
            if (position.NodeType != NodeType.Position)
                throw new BusinessValidationException("Сотрудника можно снять только с должности");

            position.EmployeeId = null;
            position.HireDate = null;

            unitOfWork.StructureNodes.Update(position);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteEmployeeAsync(int employeeId)
        {
            await using var unitOfWork = new UnitOfWork(_contextFactory);
            var employee = await unitOfWork.Employees.GetByIdAsync(employeeId) ??
                throw new BusinessValidationException("Сотрудник не найден");

            var employeeNodes = await unitOfWork.StructureNodes.GetByEmployeeIdAsync(employeeId);

            foreach (var node in employeeNodes)
            {
                node.EmployeeId = null;
                node.HireDate = null;
                unitOfWork.StructureNodes.Update(node);
            }

            unitOfWork.Employees.Delete(employee);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
