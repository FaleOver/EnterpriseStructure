using Business.Abstract;
using Common;
using Common.DTOs;
using Common.Enums;
using DataAccess.Abstract;
using DataAccess.Entities;

namespace Business.Services
{
    public class StructureService : IStructureService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StructureService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<StructureNodeDto> CreateAsync(string name, NodeType nodeType, int parentId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessValidationException("Отсутствует название");
            await ValidateParent(parentId);

            var structureNode = new StructureNode
            {
                Name = name,
                NodeType = nodeType,
                ParentId = parentId,
            };
            await _unitOfWork.StructureNodes.AddAsync(structureNode);
            await _unitOfWork.SaveChangesAsync();

            return new StructureNodeDto
            {
                Id = structureNode.Id,
                Name = structureNode.Name,
                NodeType = structureNode.NodeType,
                ParentId = structureNode.ParentId,
                EmployeeId = structureNode.EmployeeId,
                HireDate = structureNode.HireDate
            };
        }

        public async Task<IReadOnlyList<StructureNodeDto>> GetAllAsync()
        {
            var nodes = await _unitOfWork.StructureNodes.GetAllAsync();

            if (nodes.Count == 0)
                throw new BusinessValidationException("Ветки не найдены");

            var dtos = nodes.Select(n => new StructureNodeDto
            {
                Id = n.Id,
                Name = n.Name,
                NodeType = n.NodeType,
                ParentId = n.ParentId,
                EmployeeId = n.EmployeeId,
                HireDate = n.HireDate,

                Employee = n.Employee == null ? null : new EmployeeDto
                {
                    Id = n.Employee.Id,
                    LastName = n.Employee.LastName,
                    FirstName = n.Employee.FirstName,
                    MiddleName = n.Employee.MiddleName
                }
            }).ToList();

            return dtos;
        }

        public async Task<StructureNodeDto?> GetByIdAsync(int id)
        {
            var node = await _unitOfWork.StructureNodes.GetByIdAsync(id) ??
                throw new BusinessValidationException("Ветка не найдена");

            return new StructureNodeDto
            {
                Id = node.Id,
                Name = node.Name,
                NodeType = node.NodeType,
                ParentId = node.ParentId,
                EmployeeId = node.EmployeeId,
                HireDate = node.HireDate
            };
        }

        public async Task Rename(int structureNodeId, string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new BusinessValidationException("Имя не может быть пустым");

            var structureNode = await _unitOfWork.StructureNodes.GetByIdAsync(structureNodeId) ??
                throw new BusinessValidationException("Ветка не найдена");

            structureNode.Name = newName;
            _unitOfWork.StructureNodes.Update(structureNode);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var structureNode = await _unitOfWork.StructureNodes.GetByIdAsync(id) ??
                throw new BusinessValidationException("Сотрудник не найден");

            if (structureNode.ParentId != null)
            {
                await _unitOfWork.StructureNodes.DeleteAsync(structureNode);
            }
        }

        private async Task ValidateParent(int? parentId)
        {
            if (parentId == null)
                throw new BusinessValidationException("Родительский элемент обязателен");
            var parent = await _unitOfWork.StructureNodes.GetByIdAsync(parentId.Value) ?? 
                throw new BusinessValidationException("Родитель не найден");
            if (parent.NodeType == NodeType.Position)
                throw new BusinessValidationException("Должность - конечная ветка");
        }
    }
}
