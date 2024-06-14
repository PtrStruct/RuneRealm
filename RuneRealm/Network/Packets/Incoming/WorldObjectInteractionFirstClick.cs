﻿using RuneRealm.Data.ObjectsDef;
using RuneRealm.Entities;
using RuneRealm.Interactions;
using RuneRealm.Models;
using RuneRealm.Movement;
using RuneRealm.Tasks;

namespace RuneRealm.Network.Packets.Incoming;

public class WorldObjectInteractionFirstClick : IPacket
{
    private readonly int _length;
    private readonly int _opCode;
    private readonly int[,] _path;
    private readonly Player _player;
    private readonly int _x;
    private readonly int _id;
    private readonly int _y;

    public WorldObjectInteractionFirstClick(PacketParameters parameters)
    {
        _player = parameters.Player;
        _opCode = parameters.OpCode;
        _length = parameters.Length;

        var x = _player.Session.Reader.ReadSignedWordBigEndianA();
        var id = _player.Session.Reader.ReadSignedWord();
        var y = _player.Session.Reader.ReadSignedWordA();

        _x = x;
        _id = id;
        _y = y;

        var worldObject = ObjectDefinition.Lookup(_id);

        if (worldObject != null)
        {
            Console.WriteLine($"Clicked Name: {worldObject.Name} - Id: {worldObject.Id} - Width: {worldObject.Width} - Length: {worldObject.Length} - at X: {x} and Y: {y}");
            
            _player.InteractingWorldObject = new InteractingObjectModel
            {
                Id = _id,
                X = _x,
                Y = _y,
                Name = worldObject.Name,
                Width = worldObject.Width,
                Height = worldObject.Length,
                InteractionType = InteractionTypeMapping.TypeMap.GetValueOrDefault(id, InteractionType.UNKNOWN)
            };
            
            _player.InteractionHandler = InteractionHandlerFactory.GetHandler(_player.InteractingWorldObject.InteractionType);
            
            // _player.SetInteractingEntity(objectData);
        }
        else
        {
            Console.WriteLine("Object data was null..");
        }
    }

    public void Process()
    {
    }
}