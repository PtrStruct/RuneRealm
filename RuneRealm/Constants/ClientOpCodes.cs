﻿namespace RuneRealm.Constants;

public enum ClientOpCodes
{
    Idle = 0,
    FocusChange = 3,
    Chat = 4,
    ItemOnPlayer = 14,
    AlternateItemOption2 = 16,
    NpcAction2 = 17,
    NpcAction4 = 18,
    NpcAction3 = 21,
    ItemOnFloor = 25,
    MagicOnObject = 35,
    AntiCheat = 36,
    Follow = 39,
    Dialogue = 40,
    EquipItem = 41,
    Bank10Items = 43,
    FlaggedAccount = 45,
    ItemOnItem = 53,
    ItemOnNpc = 57,
    TypingOntoInterface = 60,
    ObjectAction3 = 70,
    AttackNpc = 72,
    AttackPlayer = 73,
    RemoveIgnore = 74,
    ItemAction3 = 75,
    LightItem = 79,
    CameraMovement = 86,
    DropItem = 87,
    PrivacyOptions = 95,
    WalkOnCommand = 98,
    DesignScreen = 101,
    PlayerCommand = 103,
    Bank5Items = 117,
    LoadingFinished = 121,
    ItemAction1 = 122,
    PrivateMessage = 126,
    AcceptChallenge = 128,
    BankAllItems = 129,
    CloseWindow = 130,
    MagicOnNpc = 131,
    ObjectAction1 = 132,
    AddIgnore = 133,
    BankXItemsPart1 = 135,
    TradeRequest = 139,
    UnequipItem = 145,
    PlayerOption2 = 153,
    NpcAction1 = 155,
    RegularWalk = 164,
    MagicOnGroundItem = 181,
    ButtonClick = 185,
    AddFriend = 188,
    ItemOnObject = 192,
    IdleLogout = 202,
    BankXItemsPart2 = 208,
    RegionChange = 210,
    MoveItem = 214,
    RemoveFriend = 215,
    ReportPlayer = 218,
    ObjectOption4 = 228,
    ObjectOption2 = 234,
    PickupGroundItem = 236,
    MagicOnItems = 237,
    MouseClick = 241,
    MapWalk = 248,
    MagicOnPlayer = 249,
    GroundItemAction = 253
}
