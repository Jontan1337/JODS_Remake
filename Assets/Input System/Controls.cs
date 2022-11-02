// GENERATED AUTOMATICALLY FROM 'Assets/Input System/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Controls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Master"",
            ""id"": ""fe6537ac-7067-45ec-9ecd-18f23f135d24"",
            ""actions"": [
                {
                    ""name"": ""LMB"",
                    ""type"": ""Button"",
                    ""id"": ""2b33c84b-11c8-4445-8f33-024d8771e93d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RMB"",
                    ""type"": ""Button"",
                    ""id"": ""77365dda-ee6e-4d1a-9bc4-c96bbbe09d14"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Movement"",
                    ""type"": ""PassThrough"",
                    ""id"": ""9d6849ac-aa2d-4ffd-b982-311bedd9c4e3"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Floor Up"",
                    ""type"": ""Button"",
                    ""id"": ""968763e4-0606-4865-b387-a5746d12c85d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Floor Down"",
                    ""type"": ""Button"",
                    ""id"": ""2de6d8d2-31a1-49d0-8e32-7b60311d7396"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Unit Selecting"",
                    ""type"": ""Value"",
                    ""id"": ""c6f6740d-aeaf-46e4-9588-e027f3af2e17"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Shift"",
                    ""type"": ""Button"",
                    ""id"": ""10f67212-b14e-4a14-b9ab-c6bb61a1beb4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Ctrl"",
                    ""type"": ""Button"",
                    ""id"": ""5e23018f-9896-4e2e-88ed-297b282c5d45"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Change Camera"",
                    ""type"": ""Button"",
                    ""id"": ""4b0d47ca-486b-47f8-b20d-f2d43926bc19"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Camera"",
                    ""type"": ""PassThrough"",
                    ""id"": ""ca103899-8dc3-47fd-9493-02b164cc3c62"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Alt"",
                    ""type"": ""Button"",
                    ""id"": ""d7f199cc-da7e-47b5-99a7-55d7cc4c28f5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Take Control"",
                    ""type"": ""Button"",
                    ""id"": ""8614edfd-5e00-4a96-8dff-bd3292ff3c43"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Open Unit Upgrade Menu"",
                    ""type"": ""Button"",
                    ""id"": ""700fbe75-3c74-4aa8-8efb-0485e8be586e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Open Master Upgrade Menu"",
                    ""type"": ""Button"",
                    ""id"": ""776dae05-1d19-4624-9b69-6bd5f37c234a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Scroll Wheel"",
                    ""type"": ""Value"",
                    ""id"": ""4c01f54c-6ffc-4624-917e-5ad065e75dd1"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""a6da5980-139d-4c6b-8368-783ffdf647b2"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Game Actions"",
                    ""action"": ""LMB"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""46ac3df0-e287-40c9-9897-5964612e564e"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Game Actions"",
                    ""action"": ""RMB"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""78fa70b8-6f19-4cc5-8bf4-6b46538b8e0e"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Game Actions"",
                    ""action"": ""Floor Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""da4b7523-adf5-4e7b-8869-8f7e066591c3"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Game Actions"",
                    ""action"": ""Floor Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b52daaf4-72e4-4aec-95ea-afa8135dbe30"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": ""Scale"",
                    ""groups"": """",
                    ""action"": ""Unit Selecting"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e06673f6-4d68-4b3f-befa-c38fa8d75d53"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=2)"",
                    ""groups"": """",
                    ""action"": ""Unit Selecting"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1fbd69be-3bc8-4145-888d-12609aa309f8"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=3)"",
                    ""groups"": """",
                    ""action"": ""Unit Selecting"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cf5cbc0a-40b0-43a9-9454-3d6d620619ec"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=4)"",
                    ""groups"": """",
                    ""action"": ""Unit Selecting"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c5fcbffe-02b6-4ade-a94f-1a86d01bc649"",
                    ""path"": ""<Keyboard>/5"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=5)"",
                    ""groups"": """",
                    ""action"": ""Unit Selecting"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f015a8d9-0a67-40de-be31-6c0f8b5c30a6"",
                    ""path"": ""<Keyboard>/6"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=6)"",
                    ""groups"": """",
                    ""action"": ""Unit Selecting"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8e5e1ac4-7191-42c9-8451-8bd4487023e4"",
                    ""path"": ""<Keyboard>/7"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=7)"",
                    ""groups"": """",
                    ""action"": ""Unit Selecting"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a7f55566-df88-49ac-bb2d-bb1d09058460"",
                    ""path"": ""<Keyboard>/8"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=8)"",
                    ""groups"": """",
                    ""action"": ""Unit Selecting"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""328e1793-8281-42e1-8f13-4740fab0832b"",
                    ""path"": ""<Keyboard>/9"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=9)"",
                    ""groups"": """",
                    ""action"": ""Unit Selecting"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""370b2f35-2f63-44ea-b2fe-ca02ea759d9a"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shift"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d45f1b20-3ba3-44c4-b108-95058661f1f5"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ctrl"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a66c754e-7a44-4c4a-aa94-b7fb63b5f91b"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Change Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b4c52c66-46dd-412c-8022-5a2f8424b32d"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6c7cf805-2f36-4601-a527-95179013578b"",
                    ""path"": ""<Keyboard>/leftAlt"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Alt"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9486edfc-d91c-4084-a42d-f02419f99845"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Take Control"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""d56a714e-9096-4be9-8779-66e109ae1d0c"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""6e443704-dfe8-43e8-9d15-efaedf249e4c"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""0d4d2949-f394-495f-b5d5-5f29d1717a5d"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""fcb94148-a1d5-4a64-ad2a-1cbb0d5d1763"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""a44c9dfa-b707-4776-b154-6aa6faa2bf36"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""6ccb71b4-b635-4017-a2bb-c66b128851e5"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Open Unit Upgrade Menu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""044c16aa-efe8-49b5-a296-ae0206b0930d"",
                    ""path"": ""<Keyboard>/t"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Open Master Upgrade Menu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""33b61952-eff7-4d65-a9ca-84b7e6d56031"",
                    ""path"": ""<Mouse>/scroll"",
                    ""interactions"": """",
                    ""processors"": ""NormalizeVector2"",
                    ""groups"": """",
                    ""action"": ""Scroll Wheel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Survivor"",
            ""id"": ""5099cd33-2d13-4876-92a2-34fd78f0c0a5"",
            ""actions"": [
                {
                    ""name"": ""LMB"",
                    ""type"": ""PassThrough"",
                    ""id"": ""9846ef2e-34d6-48a5-b5d3-070dc33f2dc4"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""RMB"",
                    ""type"": ""PassThrough"",
                    ""id"": ""99caf4ec-47ad-4912-ad28-e7983ef0e694"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""Movement"",
                    ""type"": ""PassThrough"",
                    ""id"": ""72938724-e4c3-4702-aa21-5240b7a6aa7c"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""d4646ee5-72f9-4108-bc62-51785ef39e68"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Camera"",
                    ""type"": ""PassThrough"",
                    ""id"": ""61c3a70e-68ea-4ce3-b203-4fd128c11e2d"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Reload"",
                    ""type"": ""PassThrough"",
                    ""id"": ""98d2bbe1-9329-4b3d-b936-d0ade65520cc"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""PassThrough"",
                    ""id"": ""f411bafe-6630-4353-a27a-980dcbc3d809"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""Drop"",
                    ""type"": ""Button"",
                    ""id"": ""002ab637-4bee-4e77-9170-5e20e335752d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Hotbar selecting"",
                    ""type"": ""Value"",
                    ""id"": ""f88205a3-c0e2-4aff-ab88-28a9b10f9d75"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Active Ability"",
                    ""type"": ""Button"",
                    ""id"": ""578afbf4-26c3-4e96-9611-07f3e12f1d61"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Sprint"",
                    ""type"": ""Button"",
                    ""id"": ""15077264-2949-43c7-a405-37d2c0121018"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Change fire mode"",
                    ""type"": ""Button"",
                    ""id"": ""45d8c89c-a966-4ff9-8ee3-10f1bf18fb8e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""f24f344d-813b-41aa-8679-667289bff570"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""ad7e8338-5a4e-4c5f-a72e-032881837790"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""8a004cef-c01e-4173-98cc-6d46acc17b89"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""f962f614-406d-4bea-a58c-c41b6a529031"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""055e9154-8cc6-491f-a516-1d5b134ea694"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""a11fc30e-a95f-46d8-93e4-1802c33f2066"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9d7a9463-9386-4f22-9ff6-a0ae13895675"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c00fd729-8050-4e98-bd08-d73ebdd954ef"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9d5bf31f-d947-4a59-86b8-7fef1ae32c3e"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": ""Scale"",
                    ""groups"": """",
                    ""action"": ""Hotbar selecting"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7ec4747a-4130-408a-8acf-1159cafc0ccf"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=2)"",
                    ""groups"": """",
                    ""action"": ""Hotbar selecting"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ca81dc22-6755-4083-a53b-d568850f5b90"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=3)"",
                    ""groups"": """",
                    ""action"": ""Hotbar selecting"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""97fc0599-9a0e-418e-a97a-f459ccbe6738"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=4)"",
                    ""groups"": """",
                    ""action"": ""Hotbar selecting"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bf1f48ce-540d-4546-9133-e90fa2febfc7"",
                    ""path"": ""<Keyboard>/5"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=5)"",
                    ""groups"": """",
                    ""action"": ""Hotbar selecting"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2881c5d9-695c-47b2-9ada-5161e55f8e4d"",
                    ""path"": ""<Keyboard>/6"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=6)"",
                    ""groups"": """",
                    ""action"": ""Hotbar selecting"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3b6786a3-75b3-4633-9aea-8900fe3ab79d"",
                    ""path"": ""<Keyboard>/7"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=7)"",
                    ""groups"": """",
                    ""action"": ""Hotbar selecting"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d9f5aefd-6eb5-44f2-9ace-93d4a3283019"",
                    ""path"": ""<Keyboard>/8"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=8)"",
                    ""groups"": """",
                    ""action"": ""Hotbar selecting"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f953cf2d-6b39-4630-abb1-8fb9e6020bb5"",
                    ""path"": ""<Keyboard>/9"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=9)"",
                    ""groups"": """",
                    ""action"": ""Hotbar selecting"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""adcb9946-3766-46d1-bcab-5de4aa89358e"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Active Ability"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ffc28ef2-1baa-465c-aa95-fd558e9f89db"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b1e454e4-7e89-4175-9b93-91e01e8a7dae"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Reload"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f4ed90bc-0822-4a22-a91d-a523c53ae7e1"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Drop"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8116cd39-96d0-48e9-91c0-4f32fef1a30a"",
                    ""path"": ""<Keyboard>/b"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Change fire mode"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5f9346c5-418b-4004-98ca-6e09753d9145"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Game Actions"",
                    ""action"": ""LMB"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7b5ffd5c-da12-4d51-b118-0ca891118039"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Game Actions"",
                    ""action"": ""RMB"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Main Menu"",
            ""id"": ""cbf3e55b-31a0-4f7c-abdf-8ed2b09a6040"",
            ""actions"": [
                {
                    ""name"": ""LMB"",
                    ""type"": ""Button"",
                    ""id"": ""caadbba8-dba2-4cb9-80af-7fc723446bdb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Space"",
                    ""type"": ""Button"",
                    ""id"": ""8eebb104-479d-4ccc-8359-69473b8b69fb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Escape"",
                    ""type"": ""Button"",
                    ""id"": ""89f4137b-3367-4219-b838-4daf119b59e4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""8abfd830-c3db-487e-bae4-be2b7dc3afd0"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LMB"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0fb8f5d9-f7a0-40e4-a23d-74185244e47f"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Space"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d7eba40a-6f9d-405d-b5ef-3d42da43f6e9"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Escape"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""General"",
            ""id"": ""f0be7ca1-c141-4fd6-945f-f7b2368dc944"",
            ""actions"": [
                {
                    ""name"": ""MouseDelta"",
                    ""type"": ""PassThrough"",
                    ""id"": ""8466f367-d8b3-4ae1-a878-768a2837eeae"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Movement"",
                    ""type"": ""PassThrough"",
                    ""id"": ""8ffeebc4-3b0a-46bf-b5f2-b7f9717cc933"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Scoreboard"",
                    ""type"": ""Button"",
                    ""id"": ""64170f68-8242-49bf-ada7-adf2646080de"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Chat"",
                    ""type"": ""Button"",
                    ""id"": ""4fb25f25-5ff4-490c-947a-f933b4d220a4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""7ecb809c-9257-4684-9c29-9357b803c565"",
                    ""path"": ""<Keyboard>/leftAlt"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Scoreboard"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c6b49c98-2d50-4ebc-99be-d3e8685aa572"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseDelta"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""0242b4e6-c656-4b7d-901d-8e142c56e012"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""a425af82-cf0a-4096-a6d8-d692a86ee3e8"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""2a46bddd-d5a5-423f-8060-c31e8f19d14a"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""90cbd5a1-5358-4d9c-81cc-2d59387ef21e"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""54168f7d-f428-491d-81a5-986d6ebf62ba"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""63b66966-df62-4b4a-aed2-a957a4c552c3"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Chat"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Game Actions"",
            ""bindingGroup"": ""Game Actions"",
            ""devices"": []
        }
    ]
}");
        // Master
        m_Master = asset.FindActionMap("Master", throwIfNotFound: true);
        m_Master_LMB = m_Master.FindAction("LMB", throwIfNotFound: true);
        m_Master_RMB = m_Master.FindAction("RMB", throwIfNotFound: true);
        m_Master_Movement = m_Master.FindAction("Movement", throwIfNotFound: true);
        m_Master_FloorUp = m_Master.FindAction("Floor Up", throwIfNotFound: true);
        m_Master_FloorDown = m_Master.FindAction("Floor Down", throwIfNotFound: true);
        m_Master_UnitSelecting = m_Master.FindAction("Unit Selecting", throwIfNotFound: true);
        m_Master_Shift = m_Master.FindAction("Shift", throwIfNotFound: true);
        m_Master_Ctrl = m_Master.FindAction("Ctrl", throwIfNotFound: true);
        m_Master_ChangeCamera = m_Master.FindAction("Change Camera", throwIfNotFound: true);
        m_Master_Camera = m_Master.FindAction("Camera", throwIfNotFound: true);
        m_Master_Alt = m_Master.FindAction("Alt", throwIfNotFound: true);
        m_Master_TakeControl = m_Master.FindAction("Take Control", throwIfNotFound: true);
        m_Master_OpenUnitUpgradeMenu = m_Master.FindAction("Open Unit Upgrade Menu", throwIfNotFound: true);
        m_Master_OpenMasterUpgradeMenu = m_Master.FindAction("Open Master Upgrade Menu", throwIfNotFound: true);
        m_Master_ScrollWheel = m_Master.FindAction("Scroll Wheel", throwIfNotFound: true);
        // Survivor
        m_Survivor = asset.FindActionMap("Survivor", throwIfNotFound: true);
        m_Survivor_LMB = m_Survivor.FindAction("LMB", throwIfNotFound: true);
        m_Survivor_RMB = m_Survivor.FindAction("RMB", throwIfNotFound: true);
        m_Survivor_Movement = m_Survivor.FindAction("Movement", throwIfNotFound: true);
        m_Survivor_Jump = m_Survivor.FindAction("Jump", throwIfNotFound: true);
        m_Survivor_Camera = m_Survivor.FindAction("Camera", throwIfNotFound: true);
        m_Survivor_Reload = m_Survivor.FindAction("Reload", throwIfNotFound: true);
        m_Survivor_Interact = m_Survivor.FindAction("Interact", throwIfNotFound: true);
        m_Survivor_Drop = m_Survivor.FindAction("Drop", throwIfNotFound: true);
        m_Survivor_Hotbarselecting = m_Survivor.FindAction("Hotbar selecting", throwIfNotFound: true);
        m_Survivor_ActiveAbility = m_Survivor.FindAction("Active Ability", throwIfNotFound: true);
        m_Survivor_Sprint = m_Survivor.FindAction("Sprint", throwIfNotFound: true);
        m_Survivor_Changefiremode = m_Survivor.FindAction("Change fire mode", throwIfNotFound: true);
        // Main Menu
        m_MainMenu = asset.FindActionMap("Main Menu", throwIfNotFound: true);
        m_MainMenu_LMB = m_MainMenu.FindAction("LMB", throwIfNotFound: true);
        m_MainMenu_Space = m_MainMenu.FindAction("Space", throwIfNotFound: true);
        m_MainMenu_Escape = m_MainMenu.FindAction("Escape", throwIfNotFound: true);
        // General
        m_General = asset.FindActionMap("General", throwIfNotFound: true);
        m_General_MouseDelta = m_General.FindAction("MouseDelta", throwIfNotFound: true);
        m_General_Movement = m_General.FindAction("Movement", throwIfNotFound: true);
        m_General_Scoreboard = m_General.FindAction("Scoreboard", throwIfNotFound: true);
        m_General_Chat = m_General.FindAction("Chat", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Master
    private readonly InputActionMap m_Master;
    private IMasterActions m_MasterActionsCallbackInterface;
    private readonly InputAction m_Master_LMB;
    private readonly InputAction m_Master_RMB;
    private readonly InputAction m_Master_Movement;
    private readonly InputAction m_Master_FloorUp;
    private readonly InputAction m_Master_FloorDown;
    private readonly InputAction m_Master_UnitSelecting;
    private readonly InputAction m_Master_Shift;
    private readonly InputAction m_Master_Ctrl;
    private readonly InputAction m_Master_ChangeCamera;
    private readonly InputAction m_Master_Camera;
    private readonly InputAction m_Master_Alt;
    private readonly InputAction m_Master_TakeControl;
    private readonly InputAction m_Master_OpenUnitUpgradeMenu;
    private readonly InputAction m_Master_OpenMasterUpgradeMenu;
    private readonly InputAction m_Master_ScrollWheel;
    public struct MasterActions
    {
        private @Controls m_Wrapper;
        public MasterActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @LMB => m_Wrapper.m_Master_LMB;
        public InputAction @RMB => m_Wrapper.m_Master_RMB;
        public InputAction @Movement => m_Wrapper.m_Master_Movement;
        public InputAction @FloorUp => m_Wrapper.m_Master_FloorUp;
        public InputAction @FloorDown => m_Wrapper.m_Master_FloorDown;
        public InputAction @UnitSelecting => m_Wrapper.m_Master_UnitSelecting;
        public InputAction @Shift => m_Wrapper.m_Master_Shift;
        public InputAction @Ctrl => m_Wrapper.m_Master_Ctrl;
        public InputAction @ChangeCamera => m_Wrapper.m_Master_ChangeCamera;
        public InputAction @Camera => m_Wrapper.m_Master_Camera;
        public InputAction @Alt => m_Wrapper.m_Master_Alt;
        public InputAction @TakeControl => m_Wrapper.m_Master_TakeControl;
        public InputAction @OpenUnitUpgradeMenu => m_Wrapper.m_Master_OpenUnitUpgradeMenu;
        public InputAction @OpenMasterUpgradeMenu => m_Wrapper.m_Master_OpenMasterUpgradeMenu;
        public InputAction @ScrollWheel => m_Wrapper.m_Master_ScrollWheel;
        public InputActionMap Get() { return m_Wrapper.m_Master; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MasterActions set) { return set.Get(); }
        public void SetCallbacks(IMasterActions instance)
        {
            if (m_Wrapper.m_MasterActionsCallbackInterface != null)
            {
                @LMB.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnLMB;
                @LMB.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnLMB;
                @LMB.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnLMB;
                @RMB.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnRMB;
                @RMB.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnRMB;
                @RMB.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnRMB;
                @Movement.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnMovement;
                @FloorUp.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnFloorUp;
                @FloorUp.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnFloorUp;
                @FloorUp.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnFloorUp;
                @FloorDown.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnFloorDown;
                @FloorDown.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnFloorDown;
                @FloorDown.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnFloorDown;
                @UnitSelecting.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnUnitSelecting;
                @UnitSelecting.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnUnitSelecting;
                @UnitSelecting.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnUnitSelecting;
                @Shift.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnShift;
                @Shift.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnShift;
                @Shift.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnShift;
                @Ctrl.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnCtrl;
                @Ctrl.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnCtrl;
                @Ctrl.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnCtrl;
                @ChangeCamera.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnChangeCamera;
                @ChangeCamera.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnChangeCamera;
                @ChangeCamera.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnChangeCamera;
                @Camera.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnCamera;
                @Camera.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnCamera;
                @Camera.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnCamera;
                @Alt.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnAlt;
                @Alt.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnAlt;
                @Alt.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnAlt;
                @TakeControl.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnTakeControl;
                @TakeControl.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnTakeControl;
                @TakeControl.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnTakeControl;
                @OpenUnitUpgradeMenu.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnOpenUnitUpgradeMenu;
                @OpenUnitUpgradeMenu.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnOpenUnitUpgradeMenu;
                @OpenUnitUpgradeMenu.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnOpenUnitUpgradeMenu;
                @OpenMasterUpgradeMenu.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnOpenMasterUpgradeMenu;
                @OpenMasterUpgradeMenu.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnOpenMasterUpgradeMenu;
                @OpenMasterUpgradeMenu.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnOpenMasterUpgradeMenu;
                @ScrollWheel.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnScrollWheel;
                @ScrollWheel.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnScrollWheel;
                @ScrollWheel.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnScrollWheel;
            }
            m_Wrapper.m_MasterActionsCallbackInterface = instance;
            if (instance != null)
            {
                @LMB.started += instance.OnLMB;
                @LMB.performed += instance.OnLMB;
                @LMB.canceled += instance.OnLMB;
                @RMB.started += instance.OnRMB;
                @RMB.performed += instance.OnRMB;
                @RMB.canceled += instance.OnRMB;
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @FloorUp.started += instance.OnFloorUp;
                @FloorUp.performed += instance.OnFloorUp;
                @FloorUp.canceled += instance.OnFloorUp;
                @FloorDown.started += instance.OnFloorDown;
                @FloorDown.performed += instance.OnFloorDown;
                @FloorDown.canceled += instance.OnFloorDown;
                @UnitSelecting.started += instance.OnUnitSelecting;
                @UnitSelecting.performed += instance.OnUnitSelecting;
                @UnitSelecting.canceled += instance.OnUnitSelecting;
                @Shift.started += instance.OnShift;
                @Shift.performed += instance.OnShift;
                @Shift.canceled += instance.OnShift;
                @Ctrl.started += instance.OnCtrl;
                @Ctrl.performed += instance.OnCtrl;
                @Ctrl.canceled += instance.OnCtrl;
                @ChangeCamera.started += instance.OnChangeCamera;
                @ChangeCamera.performed += instance.OnChangeCamera;
                @ChangeCamera.canceled += instance.OnChangeCamera;
                @Camera.started += instance.OnCamera;
                @Camera.performed += instance.OnCamera;
                @Camera.canceled += instance.OnCamera;
                @Alt.started += instance.OnAlt;
                @Alt.performed += instance.OnAlt;
                @Alt.canceled += instance.OnAlt;
                @TakeControl.started += instance.OnTakeControl;
                @TakeControl.performed += instance.OnTakeControl;
                @TakeControl.canceled += instance.OnTakeControl;
                @OpenUnitUpgradeMenu.started += instance.OnOpenUnitUpgradeMenu;
                @OpenUnitUpgradeMenu.performed += instance.OnOpenUnitUpgradeMenu;
                @OpenUnitUpgradeMenu.canceled += instance.OnOpenUnitUpgradeMenu;
                @OpenMasterUpgradeMenu.started += instance.OnOpenMasterUpgradeMenu;
                @OpenMasterUpgradeMenu.performed += instance.OnOpenMasterUpgradeMenu;
                @OpenMasterUpgradeMenu.canceled += instance.OnOpenMasterUpgradeMenu;
                @ScrollWheel.started += instance.OnScrollWheel;
                @ScrollWheel.performed += instance.OnScrollWheel;
                @ScrollWheel.canceled += instance.OnScrollWheel;
            }
        }
    }
    public MasterActions @Master => new MasterActions(this);

    // Survivor
    private readonly InputActionMap m_Survivor;
    private ISurvivorActions m_SurvivorActionsCallbackInterface;
    private readonly InputAction m_Survivor_LMB;
    private readonly InputAction m_Survivor_RMB;
    private readonly InputAction m_Survivor_Movement;
    private readonly InputAction m_Survivor_Jump;
    private readonly InputAction m_Survivor_Camera;
    private readonly InputAction m_Survivor_Reload;
    private readonly InputAction m_Survivor_Interact;
    private readonly InputAction m_Survivor_Drop;
    private readonly InputAction m_Survivor_Hotbarselecting;
    private readonly InputAction m_Survivor_ActiveAbility;
    private readonly InputAction m_Survivor_Sprint;
    private readonly InputAction m_Survivor_Changefiremode;
    public struct SurvivorActions
    {
        private @Controls m_Wrapper;
        public SurvivorActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @LMB => m_Wrapper.m_Survivor_LMB;
        public InputAction @RMB => m_Wrapper.m_Survivor_RMB;
        public InputAction @Movement => m_Wrapper.m_Survivor_Movement;
        public InputAction @Jump => m_Wrapper.m_Survivor_Jump;
        public InputAction @Camera => m_Wrapper.m_Survivor_Camera;
        public InputAction @Reload => m_Wrapper.m_Survivor_Reload;
        public InputAction @Interact => m_Wrapper.m_Survivor_Interact;
        public InputAction @Drop => m_Wrapper.m_Survivor_Drop;
        public InputAction @Hotbarselecting => m_Wrapper.m_Survivor_Hotbarselecting;
        public InputAction @ActiveAbility => m_Wrapper.m_Survivor_ActiveAbility;
        public InputAction @Sprint => m_Wrapper.m_Survivor_Sprint;
        public InputAction @Changefiremode => m_Wrapper.m_Survivor_Changefiremode;
        public InputActionMap Get() { return m_Wrapper.m_Survivor; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(SurvivorActions set) { return set.Get(); }
        public void SetCallbacks(ISurvivorActions instance)
        {
            if (m_Wrapper.m_SurvivorActionsCallbackInterface != null)
            {
                @LMB.started -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnLMB;
                @LMB.performed -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnLMB;
                @LMB.canceled -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnLMB;
                @RMB.started -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnRMB;
                @RMB.performed -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnRMB;
                @RMB.canceled -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnRMB;
                @Movement.started -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnMovement;
                @Jump.started -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnJump;
                @Camera.started -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnCamera;
                @Camera.performed -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnCamera;
                @Camera.canceled -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnCamera;
                @Reload.started -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnReload;
                @Reload.performed -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnReload;
                @Reload.canceled -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnReload;
                @Interact.started -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnInteract;
                @Drop.started -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnDrop;
                @Drop.performed -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnDrop;
                @Drop.canceled -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnDrop;
                @Hotbarselecting.started -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnHotbarselecting;
                @Hotbarselecting.performed -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnHotbarselecting;
                @Hotbarselecting.canceled -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnHotbarselecting;
                @ActiveAbility.started -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnActiveAbility;
                @ActiveAbility.performed -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnActiveAbility;
                @ActiveAbility.canceled -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnActiveAbility;
                @Sprint.started -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnSprint;
                @Sprint.performed -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnSprint;
                @Sprint.canceled -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnSprint;
                @Changefiremode.started -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnChangefiremode;
                @Changefiremode.performed -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnChangefiremode;
                @Changefiremode.canceled -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnChangefiremode;
            }
            m_Wrapper.m_SurvivorActionsCallbackInterface = instance;
            if (instance != null)
            {
                @LMB.started += instance.OnLMB;
                @LMB.performed += instance.OnLMB;
                @LMB.canceled += instance.OnLMB;
                @RMB.started += instance.OnRMB;
                @RMB.performed += instance.OnRMB;
                @RMB.canceled += instance.OnRMB;
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Camera.started += instance.OnCamera;
                @Camera.performed += instance.OnCamera;
                @Camera.canceled += instance.OnCamera;
                @Reload.started += instance.OnReload;
                @Reload.performed += instance.OnReload;
                @Reload.canceled += instance.OnReload;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @Drop.started += instance.OnDrop;
                @Drop.performed += instance.OnDrop;
                @Drop.canceled += instance.OnDrop;
                @Hotbarselecting.started += instance.OnHotbarselecting;
                @Hotbarselecting.performed += instance.OnHotbarselecting;
                @Hotbarselecting.canceled += instance.OnHotbarselecting;
                @ActiveAbility.started += instance.OnActiveAbility;
                @ActiveAbility.performed += instance.OnActiveAbility;
                @ActiveAbility.canceled += instance.OnActiveAbility;
                @Sprint.started += instance.OnSprint;
                @Sprint.performed += instance.OnSprint;
                @Sprint.canceled += instance.OnSprint;
                @Changefiremode.started += instance.OnChangefiremode;
                @Changefiremode.performed += instance.OnChangefiremode;
                @Changefiremode.canceled += instance.OnChangefiremode;
            }
        }
    }
    public SurvivorActions @Survivor => new SurvivorActions(this);

    // Main Menu
    private readonly InputActionMap m_MainMenu;
    private IMainMenuActions m_MainMenuActionsCallbackInterface;
    private readonly InputAction m_MainMenu_LMB;
    private readonly InputAction m_MainMenu_Space;
    private readonly InputAction m_MainMenu_Escape;
    public struct MainMenuActions
    {
        private @Controls m_Wrapper;
        public MainMenuActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @LMB => m_Wrapper.m_MainMenu_LMB;
        public InputAction @Space => m_Wrapper.m_MainMenu_Space;
        public InputAction @Escape => m_Wrapper.m_MainMenu_Escape;
        public InputActionMap Get() { return m_Wrapper.m_MainMenu; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MainMenuActions set) { return set.Get(); }
        public void SetCallbacks(IMainMenuActions instance)
        {
            if (m_Wrapper.m_MainMenuActionsCallbackInterface != null)
            {
                @LMB.started -= m_Wrapper.m_MainMenuActionsCallbackInterface.OnLMB;
                @LMB.performed -= m_Wrapper.m_MainMenuActionsCallbackInterface.OnLMB;
                @LMB.canceled -= m_Wrapper.m_MainMenuActionsCallbackInterface.OnLMB;
                @Space.started -= m_Wrapper.m_MainMenuActionsCallbackInterface.OnSpace;
                @Space.performed -= m_Wrapper.m_MainMenuActionsCallbackInterface.OnSpace;
                @Space.canceled -= m_Wrapper.m_MainMenuActionsCallbackInterface.OnSpace;
                @Escape.started -= m_Wrapper.m_MainMenuActionsCallbackInterface.OnEscape;
                @Escape.performed -= m_Wrapper.m_MainMenuActionsCallbackInterface.OnEscape;
                @Escape.canceled -= m_Wrapper.m_MainMenuActionsCallbackInterface.OnEscape;
            }
            m_Wrapper.m_MainMenuActionsCallbackInterface = instance;
            if (instance != null)
            {
                @LMB.started += instance.OnLMB;
                @LMB.performed += instance.OnLMB;
                @LMB.canceled += instance.OnLMB;
                @Space.started += instance.OnSpace;
                @Space.performed += instance.OnSpace;
                @Space.canceled += instance.OnSpace;
                @Escape.started += instance.OnEscape;
                @Escape.performed += instance.OnEscape;
                @Escape.canceled += instance.OnEscape;
            }
        }
    }
    public MainMenuActions @MainMenu => new MainMenuActions(this);

    // General
    private readonly InputActionMap m_General;
    private IGeneralActions m_GeneralActionsCallbackInterface;
    private readonly InputAction m_General_MouseDelta;
    private readonly InputAction m_General_Movement;
    private readonly InputAction m_General_Scoreboard;
    private readonly InputAction m_General_Chat;
    public struct GeneralActions
    {
        private @Controls m_Wrapper;
        public GeneralActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @MouseDelta => m_Wrapper.m_General_MouseDelta;
        public InputAction @Movement => m_Wrapper.m_General_Movement;
        public InputAction @Scoreboard => m_Wrapper.m_General_Scoreboard;
        public InputAction @Chat => m_Wrapper.m_General_Chat;
        public InputActionMap Get() { return m_Wrapper.m_General; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GeneralActions set) { return set.Get(); }
        public void SetCallbacks(IGeneralActions instance)
        {
            if (m_Wrapper.m_GeneralActionsCallbackInterface != null)
            {
                @MouseDelta.started -= m_Wrapper.m_GeneralActionsCallbackInterface.OnMouseDelta;
                @MouseDelta.performed -= m_Wrapper.m_GeneralActionsCallbackInterface.OnMouseDelta;
                @MouseDelta.canceled -= m_Wrapper.m_GeneralActionsCallbackInterface.OnMouseDelta;
                @Movement.started -= m_Wrapper.m_GeneralActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_GeneralActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_GeneralActionsCallbackInterface.OnMovement;
                @Scoreboard.started -= m_Wrapper.m_GeneralActionsCallbackInterface.OnScoreboard;
                @Scoreboard.performed -= m_Wrapper.m_GeneralActionsCallbackInterface.OnScoreboard;
                @Scoreboard.canceled -= m_Wrapper.m_GeneralActionsCallbackInterface.OnScoreboard;
                @Chat.started -= m_Wrapper.m_GeneralActionsCallbackInterface.OnChat;
                @Chat.performed -= m_Wrapper.m_GeneralActionsCallbackInterface.OnChat;
                @Chat.canceled -= m_Wrapper.m_GeneralActionsCallbackInterface.OnChat;
            }
            m_Wrapper.m_GeneralActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MouseDelta.started += instance.OnMouseDelta;
                @MouseDelta.performed += instance.OnMouseDelta;
                @MouseDelta.canceled += instance.OnMouseDelta;
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Scoreboard.started += instance.OnScoreboard;
                @Scoreboard.performed += instance.OnScoreboard;
                @Scoreboard.canceled += instance.OnScoreboard;
                @Chat.started += instance.OnChat;
                @Chat.performed += instance.OnChat;
                @Chat.canceled += instance.OnChat;
            }
        }
    }
    public GeneralActions @General => new GeneralActions(this);
    private int m_GameActionsSchemeIndex = -1;
    public InputControlScheme GameActionsScheme
    {
        get
        {
            if (m_GameActionsSchemeIndex == -1) m_GameActionsSchemeIndex = asset.FindControlSchemeIndex("Game Actions");
            return asset.controlSchemes[m_GameActionsSchemeIndex];
        }
    }
    public interface IMasterActions
    {
        void OnLMB(InputAction.CallbackContext context);
        void OnRMB(InputAction.CallbackContext context);
        void OnMovement(InputAction.CallbackContext context);
        void OnFloorUp(InputAction.CallbackContext context);
        void OnFloorDown(InputAction.CallbackContext context);
        void OnUnitSelecting(InputAction.CallbackContext context);
        void OnShift(InputAction.CallbackContext context);
        void OnCtrl(InputAction.CallbackContext context);
        void OnChangeCamera(InputAction.CallbackContext context);
        void OnCamera(InputAction.CallbackContext context);
        void OnAlt(InputAction.CallbackContext context);
        void OnTakeControl(InputAction.CallbackContext context);
        void OnOpenUnitUpgradeMenu(InputAction.CallbackContext context);
        void OnOpenMasterUpgradeMenu(InputAction.CallbackContext context);
        void OnScrollWheel(InputAction.CallbackContext context);
    }
    public interface ISurvivorActions
    {
        void OnLMB(InputAction.CallbackContext context);
        void OnRMB(InputAction.CallbackContext context);
        void OnMovement(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnCamera(InputAction.CallbackContext context);
        void OnReload(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnDrop(InputAction.CallbackContext context);
        void OnHotbarselecting(InputAction.CallbackContext context);
        void OnActiveAbility(InputAction.CallbackContext context);
        void OnSprint(InputAction.CallbackContext context);
        void OnChangefiremode(InputAction.CallbackContext context);
    }
    public interface IMainMenuActions
    {
        void OnLMB(InputAction.CallbackContext context);
        void OnSpace(InputAction.CallbackContext context);
        void OnEscape(InputAction.CallbackContext context);
    }
    public interface IGeneralActions
    {
        void OnMouseDelta(InputAction.CallbackContext context);
        void OnMovement(InputAction.CallbackContext context);
        void OnScoreboard(InputAction.CallbackContext context);
        void OnChat(InputAction.CallbackContext context);
    }
}
