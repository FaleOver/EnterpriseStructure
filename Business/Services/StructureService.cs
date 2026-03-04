using Business.Abstract;
using Common;
using Common.DTOs;
using Common.Enums;
using DataAccess;
using DataAccess.Abstract;
using DataAccess.Entities;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Business.Services
{
    public class StructureService : IStructureService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public StructureService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<StructureNodeDto> CreateAsync(string name, NodeType nodeType, int parentId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessValidationException("Отсутствует название");
            
            await using var unitOfWork = new UnitOfWork(_contextFactory);
            await ValidateParent(parentId, unitOfWork);

            var structureNode = new StructureNode
            {
                Name = name,
                NodeType = nodeType,
                ParentId = parentId,
            };

            await unitOfWork.StructureNodes.AddAsync(structureNode);
            await unitOfWork.SaveChangesAsync();

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
            IReadOnlyList<StructureNode> nodes;
            await using var unitOfWork = new UnitOfWork(_contextFactory);
            nodes = await unitOfWork.StructureNodes.GetAllAsync();

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
            StructureNode node;
            await using var unitOfWork = new UnitOfWork(_contextFactory);
            node = await unitOfWork.StructureNodes.GetByIdAsync(id) ??
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

            StructureNode structureNode;
            await using var unitOfWork = new UnitOfWork(_contextFactory);
            structureNode = await unitOfWork.StructureNodes.GetByIdAsync(structureNodeId) ??
            throw new BusinessValidationException("Ветка не найдена");

            structureNode.Name = newName;
            unitOfWork.StructureNodes.Update(structureNode);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await using var unitOfWork = new UnitOfWork(_contextFactory);
            var structureNode = await unitOfWork.StructureNodes.GetByIdAsync(id) ??
                throw new BusinessValidationException("Ветка не найдена");

            if (structureNode.ParentId != null)
                await unitOfWork.StructureNodes.DeleteAsync(structureNode);
            else
                throw new BusinessValidationException("Корневой узей нельзя удалить");
        }

        private async Task ValidateParent(int? parentId, IUnitOfWork unitOfWork)
        {
            if (parentId == null)
                throw new BusinessValidationException("Родительский элемент обязателен");
            var parent = await unitOfWork.StructureNodes.GetByIdAsync(parentId.Value) ?? 
                throw new BusinessValidationException("Родитель не найден");
            if (parent.NodeType == NodeType.Position)
                throw new BusinessValidationException("Должность - конечная ветка");
        }
    }
}
