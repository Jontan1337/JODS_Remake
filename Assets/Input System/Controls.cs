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
                    ""name"": ""Shift LMB (doesn't work)"",
                    ""type"": ""Button"",
                    ""id"": ""8235a6ea-b79d-4993-9db5-17c2abd056cc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Shift RMB (doesn't work)"",
                    ""type"": ""Button"",
                    ""id"": ""1822b38b-cfb5-4875-8226-21d29d505d7e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Ctrl LMB (doesn't work)"",
                    ""type"": ""Button"",
                    ""id"": ""c5b6d656-41a5-4cdc-b29b-e43982495654"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Ctrl RMB (doesn't work)"",
                    ""type"": ""Button"",
                    ""id"": ""dc7ebe05-3d8f-4f5f-9ebf-8c62fab270b6"",
                    ""expectedControlType"": ""Button"",
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
                    ""name"": ""Movement"",
                    ""type"": ""PassThrough"",
                    ""id"": ""9d6849ac-aa2d-4ffd-b982-311bedd9c4e3"",
                    ""expectedControlType"": ""Vector2"",
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
                    ""name"": ""Button With One Modifier"",
                    ""id"": ""3916c2e7-ac2a-4bb1-ab6b-ba17855846ac"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shift LMB (doesn't work)"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""648c7841-c451-40c1-ba3c-efa2a6fcbcf3"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shift LMB (doesn't work)"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""c406ccc7-2ba0-42be-887f-bfb73744cb87"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shift LMB (doesn't work)"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Button With One Modifier"",
                    ""id"": ""e59494a4-1dc1-411d-b4aa-a6d9c685ac05"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shift RMB (doesn't work)"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""36dd71d4-9c43-4627-b295-08362005c3b0"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shift RMB (doesn't work)"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""f5a996b7-e75e-4d97-afc7-bc391f452c53"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shift RMB (doesn't work)"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Button With One Modifier"",
                    ""id"": ""e9960491-ce25-4931-a1be-9e6f0196ebbd"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ctrl LMB (doesn't work)"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""67138e63-d517-4bf7-a591-30f3bcabe69b"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ctrl LMB (doesn't work)"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""c5c18ef8-86ff-4a4c-aaa8-8a47b7590cd4"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ctrl LMB (doesn't work)"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Button With One Modifier"",
                    ""id"": ""f732fdfa-2311-4dce-bf57-934c9ee857d8"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ctrl RMB (doesn't work)"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""caa2a249-ca59-4349-ad92-c08a0965b29d"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ctrl RMB (doesn't work)"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""47dffaa7-52c4-4713-898d-7b2e61443c0b"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ctrl RMB (doesn't work)"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
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
                }
            ]
        },
        {
            ""name"": ""Survivor"",
            ""id"": ""5099cd33-2d13-4876-92a2-34fd78f0c0a5"",
            ""actions"": [
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
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""f411bafe-6630-4353-a27a-980dcbc3d809"",
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
                    ""name"": ""bleb"",
                    ""type"": ""Button"",
                    ""id"": ""c11c669e-355b-4583-95c7-5d488628cac0"",
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
                    ""id"": ""7306b8f7-52ba-4c0b-a510-4573edff83eb"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""bleb"",
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
        m_Master_ShiftLMBdoesntwork = m_Master.FindAction("Shift LMB (doesn't work)", throwIfNotFound: true);
        m_Master_ShiftRMBdoesntwork = m_Master.FindAction("Shift RMB (doesn't work)", throwIfNotFound: true);
        m_Master_CtrlLMBdoesntwork = m_Master.FindAction("Ctrl LMB (doesn't work)", throwIfNotFound: true);
        m_Master_CtrlRMBdoesntwork = m_Master.FindAction("Ctrl RMB (doesn't work)", throwIfNotFound: true);
        m_Master_FloorUp = m_Master.FindAction("Floor Up", throwIfNotFound: true);
        m_Master_FloorDown = m_Master.FindAction("Floor Down", throwIfNotFound: true);
        m_Master_UnitSelecting = m_Master.FindAction("Unit Selecting", throwIfNotFound: true);
        m_Master_Movement = m_Master.FindAction("Movement", throwIfNotFound: true);
        m_Master_Shift = m_Master.FindAction("Shift", throwIfNotFound: true);
        m_Master_Ctrl = m_Master.FindAction("Ctrl", throwIfNotFound: true);
        m_Master_ChangeCamera = m_Master.FindAction("Change Camera", throwIfNotFound: true);
        m_Master_Camera = m_Master.FindAction("Camera", throwIfNotFound: true);
        // Survivor
        m_Survivor = asset.FindActionMap("Survivor", throwIfNotFound: true);
        m_Survivor_Movement = m_Survivor.FindAction("Movement", throwIfNotFound: true);
        m_Survivor_Jump = m_Survivor.FindAction("Jump", throwIfNotFound: true);
        m_Survivor_Camera = m_Survivor.FindAction("Camera", throwIfNotFound: true);
        m_Survivor_Interact = m_Survivor.FindAction("Interact", throwIfNotFound: true);
        m_Survivor_Hotbarselecting = m_Survivor.FindAction("Hotbar selecting", throwIfNotFound: true);
        m_Survivor_bleb = m_Survivor.FindAction("bleb", throwIfNotFound: true);
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
    private readonly InputAction m_Master_ShiftLMBdoesntwork;
    private readonly InputAction m_Master_ShiftRMBdoesntwork;
    private readonly InputAction m_Master_CtrlLMBdoesntwork;
    private readonly InputAction m_Master_CtrlRMBdoesntwork;
    private readonly InputAction m_Master_FloorUp;
    private readonly InputAction m_Master_FloorDown;
    private readonly InputAction m_Master_UnitSelecting;
    private readonly InputAction m_Master_Movement;
    private readonly InputAction m_Master_Shift;
    private readonly InputAction m_Master_Ctrl;
    private readonly InputAction m_Master_ChangeCamera;
    private readonly InputAction m_Master_Camera;
    public struct MasterActions
    {
        private @Controls m_Wrapper;
        public MasterActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @LMB => m_Wrapper.m_Master_LMB;
        public InputAction @RMB => m_Wrapper.m_Master_RMB;
        public InputAction @ShiftLMBdoesntwork => m_Wrapper.m_Master_ShiftLMBdoesntwork;
        public InputAction @ShiftRMBdoesntwork => m_Wrapper.m_Master_ShiftRMBdoesntwork;
        public InputAction @CtrlLMBdoesntwork => m_Wrapper.m_Master_CtrlLMBdoesntwork;
        public InputAction @CtrlRMBdoesntwork => m_Wrapper.m_Master_CtrlRMBdoesntwork;
        public InputAction @FloorUp => m_Wrapper.m_Master_FloorUp;
        public InputAction @FloorDown => m_Wrapper.m_Master_FloorDown;
        public InputAction @UnitSelecting => m_Wrapper.m_Master_UnitSelecting;
        public InputAction @Movement => m_Wrapper.m_Master_Movement;
        public InputAction @Shift => m_Wrapper.m_Master_Shift;
        public InputAction @Ctrl => m_Wrapper.m_Master_Ctrl;
        public InputAction @ChangeCamera => m_Wrapper.m_Master_ChangeCamera;
        public InputAction @Camera => m_Wrapper.m_Master_Camera;
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
                @ShiftLMBdoesntwork.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnShiftLMBdoesntwork;
                @ShiftLMBdoesntwork.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnShiftLMBdoesntwork;
                @ShiftLMBdoesntwork.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnShiftLMBdoesntwork;
                @ShiftRMBdoesntwork.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnShiftRMBdoesntwork;
                @ShiftRMBdoesntwork.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnShiftRMBdoesntwork;
                @ShiftRMBdoesntwork.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnShiftRMBdoesntwork;
                @CtrlLMBdoesntwork.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnCtrlLMBdoesntwork;
                @CtrlLMBdoesntwork.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnCtrlLMBdoesntwork;
                @CtrlLMBdoesntwork.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnCtrlLMBdoesntwork;
                @CtrlRMBdoesntwork.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnCtrlRMBdoesntwork;
                @CtrlRMBdoesntwork.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnCtrlRMBdoesntwork;
                @CtrlRMBdoesntwork.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnCtrlRMBdoesntwork;
                @FloorUp.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnFloorUp;
                @FloorUp.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnFloorUp;
                @FloorUp.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnFloorUp;
                @FloorDown.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnFloorDown;
                @FloorDown.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnFloorDown;
                @FloorDown.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnFloorDown;
                @UnitSelecting.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnUnitSelecting;
                @UnitSelecting.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnUnitSelecting;
                @UnitSelecting.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnUnitSelecting;
                @Movement.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnMovement;
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
                @ShiftLMBdoesntwork.started += instance.OnShiftLMBdoesntwork;
                @ShiftLMBdoesntwork.performed += instance.OnShiftLMBdoesntwork;
                @ShiftLMBdoesntwork.canceled += instance.OnShiftLMBdoesntwork;
                @ShiftRMBdoesntwork.started += instance.OnShiftRMBdoesntwork;
                @ShiftRMBdoesntwork.performed += instance.OnShiftRMBdoesntwork;
                @ShiftRMBdoesntwork.canceled += instance.OnShiftRMBdoesntwork;
                @CtrlLMBdoesntwork.started += instance.OnCtrlLMBdoesntwork;
                @CtrlLMBdoesntwork.performed += instance.OnCtrlLMBdoesntwork;
                @CtrlLMBdoesntwork.canceled += instance.OnCtrlLMBdoesntwork;
                @CtrlRMBdoesntwork.started += instance.OnCtrlRMBdoesntwork;
                @CtrlRMBdoesntwork.performed += instance.OnCtrlRMBdoesntwork;
                @CtrlRMBdoesntwork.canceled += instance.OnCtrlRMBdoesntwork;
                @FloorUp.started += instance.OnFloorUp;
                @FloorUp.performed += instance.OnFloorUp;
                @FloorUp.canceled += instance.OnFloorUp;
                @FloorDown.started += instance.OnFloorDown;
                @FloorDown.performed += instance.OnFloorDown;
                @FloorDown.canceled += instance.OnFloorDown;
                @UnitSelecting.started += instance.OnUnitSelecting;
                @UnitSelecting.performed += instance.OnUnitSelecting;
                @UnitSelecting.canceled += instance.OnUnitSelecting;
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
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
            }
        }
    }
    public MasterActions @Master => new MasterActions(this);

    // Survivor
    private readonly InputActionMap m_Survivor;
    private ISurvivorActions m_SurvivorActionsCallbackInterface;
    private readonly InputAction m_Survivor_Movement;
    private readonly InputAction m_Survivor_Jump;
    private readonly InputAction m_Survivor_Camera;
    private readonly InputAction m_Survivor_Interact;
    private readonly InputAction m_Survivor_Hotbarselecting;
    private readonly InputAction m_Survivor_bleb;
    public struct SurvivorActions
    {
        private @Controls m_Wrapper;
        public SurvivorActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Survivor_Movement;
        public InputAction @Jump => m_Wrapper.m_Survivor_Jump;
        public InputAction @Camera => m_Wrapper.m_Survivor_Camera;
        public InputAction @Interact => m_Wrapper.m_Survivor_Interact;
        public InputAction @Hotbarselecting => m_Wrapper.m_Survivor_Hotbarselecting;
        public InputAction @bleb => m_Wrapper.m_Survivor_bleb;
        public InputActionMap Get() { return m_Wrapper.m_Survivor; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(SurvivorActions set) { return set.Get(); }
        public void SetCallbacks(ISurvivorActions instance)
        {
            if (m_Wrapper.m_SurvivorActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnMovement;
                @Jump.started -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnJump;
                @Camera.started -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnCamera;
                @Camera.performed -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnCamera;
                @Camera.canceled -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnCamera;
                @Interact.started -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnInteract;
                @Hotbarselecting.started -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnHotbarselecting;
                @Hotbarselecting.performed -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnHotbarselecting;
                @Hotbarselecting.canceled -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnHotbarselecting;
                @bleb.started -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnBleb;
                @bleb.performed -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnBleb;
                @bleb.canceled -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnBleb;
            }
            m_Wrapper.m_SurvivorActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Camera.started += instance.OnCamera;
                @Camera.performed += instance.OnCamera;
                @Camera.canceled += instance.OnCamera;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @Hotbarselecting.started += instance.OnHotbarselecting;
                @Hotbarselecting.performed += instance.OnHotbarselecting;
                @Hotbarselecting.canceled += instance.OnHotbarselecting;
                @bleb.started += instance.OnBleb;
                @bleb.performed += instance.OnBleb;
                @bleb.canceled += instance.OnBleb;
            }
        }
    }
    public SurvivorActions @Survivor => new SurvivorActions(this);
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
        void OnShiftLMBdoesntwork(InputAction.CallbackContext context);
        void OnShiftRMBdoesntwork(InputAction.CallbackContext context);
        void OnCtrlLMBdoesntwork(InputAction.CallbackContext context);
        void OnCtrlRMBdoesntwork(InputAction.CallbackContext context);
        void OnFloorUp(InputAction.CallbackContext context);
        void OnFloorDown(InputAction.CallbackContext context);
        void OnUnitSelecting(InputAction.CallbackContext context);
        void OnMovement(InputAction.CallbackContext context);
        void OnShift(InputAction.CallbackContext context);
        void OnCtrl(InputAction.CallbackContext context);
        void OnChangeCamera(InputAction.CallbackContext context);
        void OnCamera(InputAction.CallbackContext context);
    }
    public interface ISurvivorActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnCamera(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnHotbarselecting(InputAction.CallbackContext context);
        void OnBleb(InputAction.CallbackContext context);
    }
}
