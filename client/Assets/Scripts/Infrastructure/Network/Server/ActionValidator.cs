using client.Assets.Scripts.Domain.Interfaces.Services;
using client.Assets.Scripts.Domain.Interfaces;
using client.Assets.Scripts.Domain.Services;
using UnityEngine;
using System;

namespace client.Assets.Scripts.Infrastructure.Network.Server
{
    public class ActionValidator
    {
        private readonly IGameContextProvider _gameContextProvider;
        private readonly IUnitsService _unitsService;
        private readonly IUnitGridInteractionService _gridInteractionService;
        private readonly ITurnService _turnService;

        public ActionValidator(
            IGameContextProvider gameContextProvider,
            IUnitsService unitsService,
            IUnitGridInteractionService gridInteractionService,
            ITurnService turnService)
        {
            _gameContextProvider = gameContextProvider;
            _unitsService = unitsService;
            _gridInteractionService = gridInteractionService;
            _turnService = turnService;
        }

        public bool ValidateMovement(Guid unitId, Vector2Int fromPosition, Vector2Int toPosition, Guid requestingPlayerId)
        {
            try
            {
                var unit = _gameContextProvider.GetUnit(unitId);
                if (unit == null)
                {
                    Debug.LogWarning($"Unit {unitId} not found");
                    return false;
                }

                if (unit.OwnerId != requestingPlayerId)
                {
                    Debug.LogWarning($"Player {requestingPlayerId} does not own unit {unitId}");
                    return false;
                }

                if (!_unitsService.CanUnitMove(unit))
                {
                    Debug.LogWarning($"Unit {unitId} cannot move (dead or other reason)");
                    return false;
                }

                var currentTurn = _gameContextProvider.GetCurrentTurn();
                if (currentTurn == null)
                {
                    Debug.LogWarning("No current turn found");
                    return false;
                }

                if (!_turnService.CanPlayerAct(currentTurn, requestingPlayerId))
                {
                    Debug.LogWarning($"Player {requestingPlayerId} cannot act in current turn");
                    return false;
                }

                if (!_turnService.CanUseAction(currentTurn, new Domain.ValueObjects.Movement()))
                {
                    Debug.LogWarning("Movement action already used this turn");
                    return false;
                }

                if (unit.Position != fromPosition)
                {
                    Debug.LogWarning($"Unit {unitId} position mismatch. Expected: {unit.Position}, Got: {fromPosition}");
                    return false;
                }

                var gameSession = _gameContextProvider.GetCurrentGameSession();
                if (gameSession == null)
                {
                    Debug.LogWarning("No current game session found");
                    return false;
                }

                var allUnits = _gameContextProvider.GetAllUnits(gameSession.SessionId);
                
                if (!_gridInteractionService.CanMoveToPosition(unit, toPosition, gameSession.Field, allUnits))
                {
                    Debug.LogWarning($"Unit {unitId} cannot move to position {toPosition}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error validating movement: {ex.Message}");
                return false;
            }
        }

        public bool ValidateAttack(Guid attackerId, Guid targetId, Vector2Int attackerPosition, Vector2Int targetPosition, Guid requestingPlayerId)
        {
            try
            {
                var attacker = _gameContextProvider.GetUnit(attackerId);
                if (attacker == null)
                {
                    Debug.LogWarning($"Attacker unit {attackerId} not found");
                    return false;
                }

                var target = _gameContextProvider.GetUnit(targetId);
                if (target == null)
                {
                    Debug.LogWarning($"Target unit {targetId} not found");
                    return false;
                }

                if (attacker.OwnerId != requestingPlayerId)
                {
                    Debug.LogWarning($"Player {requestingPlayerId} does not own attacker unit {attackerId}");
                    return false;
                }

                if (attacker.OwnerId == target.OwnerId)
                {
                    Debug.LogWarning($"Cannot attack own unit. Attacker: {attackerId}, Target: {targetId}");
                    return false;
                }

                if (!_unitsService.CanUnitAttack(attacker))
                {
                    Debug.LogWarning($"Attacker unit {attackerId} cannot attack (dead or other reason)");
                    return false;
                }

                if (!target.IsAlive)
                {
                    Debug.LogWarning($"Target unit {targetId} is already dead");
                    return false;
                }

                var currentTurn = _gameContextProvider.GetCurrentTurn();
                if (currentTurn == null)
                {
                    Debug.LogWarning("No current turn found");
                    return false;
                }

                if (!_turnService.CanPlayerAct(currentTurn, requestingPlayerId))
                {
                    Debug.LogWarning($"Player {requestingPlayerId} cannot act in current turn");
                    return false;
                }

                if (!_turnService.CanUseAction(currentTurn, new Domain.ValueObjects.Attack()))
                {
                    Debug.LogWarning("Attack action already used this turn");
                    return false;
                }

                if (attacker.Position != attackerPosition)
                {
                    Debug.LogWarning($"Attacker {attackerId} position mismatch. Expected: {attacker.Position}, Got: {attackerPosition}");
                    return false;
                }

                if (target.Position != targetPosition)
                {
                    Debug.LogWarning($"Target {targetId} position mismatch. Expected: {target.Position}, Got: {targetPosition}");
                    return false;
                }

                var gameSession = _gameContextProvider.GetCurrentGameSession();
                if (gameSession == null)
                {
                    Debug.LogWarning("No current game session found");
                    return false;
                }

                var allUnits = _gameContextProvider.GetAllUnits(gameSession.SessionId);
                
                if (!_gridInteractionService.CanAttackPosition(attacker, targetPosition, gameSession.Field, allUnits))
                {
                    Debug.LogWarning($"Unit {attackerId} cannot attack position {targetPosition}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error validating attack: {ex.Message}");
                return false;
            }
        }
    }
}