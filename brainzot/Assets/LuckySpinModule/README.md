# Lucky Spin Module

Module độc lập để quản lý Lucky Spin/Wheel system cho Unity games.

## Cấu trúc

```
LuckySpinModule/
├── Runtime/
│   ├── Core/
│   │   ├── ILuckySpinDataProvider.cs
│   │   └── LuckySpinConfig.cs
│   └── UI/
│       └── LuckySpinPopup.cs
├── Prefabs/
│   └── PopupLuckySpin.prefab
└── README.md
```

## Cách sử dụng

1. Copy module vào project
2. Tạo Config: `Create > Lucky Spin Module > Config`
3. Config rewards list trong config
4. Tạo Adapter implement `ILuckySpinDataProvider`
5. Initialize popup với adapter và config

Xem ví dụ adapter trong `Assets/030/Scripts/UI/FoodSortLuckySpinAdapter.cs`
