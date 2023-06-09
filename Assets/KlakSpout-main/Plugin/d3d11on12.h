/*** Autogenerated by WIDL 6.4 from include/d3d11on12.idl - Do not edit ***/

#ifdef _WIN32
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 475
#endif
#include <rpc.h>
#include <rpcndr.h>
#endif

#ifndef COM_NO_WINDOWS_H
#include <windows.h>
#include <ole2.h>
#endif

#ifndef __d3d11on12_h__
#define __d3d11on12_h__

/* Forward declarations */

#ifndef __ID3D11On12Device_FWD_DEFINED__
#define __ID3D11On12Device_FWD_DEFINED__
typedef interface ID3D11On12Device ID3D11On12Device;
#ifdef __cplusplus
interface ID3D11On12Device;
#endif /* __cplusplus */
#endif

/* Headers for imported files */

#include <oaidl.h>
#include <ocidl.h>
#include <d3d11.h>
#include <d3d12.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef struct D3D11_RESOURCE_FLAGS {
    UINT BindFlags;
    UINT MiscFlags;
    UINT CPUAccessFlags;
    UINT StructureByteStride;
} D3D11_RESOURCE_FLAGS;
/*****************************************************************************
 * ID3D11On12Device interface
 */
#ifndef __ID3D11On12Device_INTERFACE_DEFINED__
#define __ID3D11On12Device_INTERFACE_DEFINED__

DEFINE_GUID(IID_ID3D11On12Device, 0x85611e73, 0x70a9, 0x490e, 0x96,0x14, 0xa9,0xe3,0x02,0x77,0x79,0x04);
#if defined(__cplusplus) && !defined(CINTERFACE)
MIDL_INTERFACE("85611e73-70a9-490e-9614-a9e302777904")
ID3D11On12Device : public IUnknown
{
    virtual HRESULT STDMETHODCALLTYPE CreateWrappedResource(
        IUnknown *d3d12_resource,
        const D3D11_RESOURCE_FLAGS *flags,
        D3D12_RESOURCE_STATES input_state,
        D3D12_RESOURCE_STATES output_state,
        REFIID iid,
        void **d3d11_resource) = 0;

    virtual void STDMETHODCALLTYPE ReleaseWrappedResources(
        ID3D11Resource *const *resources,
        UINT count) = 0;

    virtual void STDMETHODCALLTYPE AcquireWrappedResources(
        ID3D11Resource *const *resources,
        UINT count) = 0;

};
#ifdef __CRT_UUID_DECL
__CRT_UUID_DECL(ID3D11On12Device, 0x85611e73, 0x70a9, 0x490e, 0x96,0x14, 0xa9,0xe3,0x02,0x77,0x79,0x04)
#endif
#else
typedef struct ID3D11On12DeviceVtbl {
    BEGIN_INTERFACE

    /*** IUnknown methods ***/
    HRESULT (STDMETHODCALLTYPE *QueryInterface)(
        ID3D11On12Device *This,
        REFIID riid,
        void **ppvObject);

    ULONG (STDMETHODCALLTYPE *AddRef)(
        ID3D11On12Device *This);

    ULONG (STDMETHODCALLTYPE *Release)(
        ID3D11On12Device *This);

    /*** ID3D11On12Device methods ***/
    HRESULT (STDMETHODCALLTYPE *CreateWrappedResource)(
        ID3D11On12Device *This,
        IUnknown *d3d12_resource,
        const D3D11_RESOURCE_FLAGS *flags,
        D3D12_RESOURCE_STATES input_state,
        D3D12_RESOURCE_STATES output_state,
        REFIID iid,
        void **d3d11_resource);

    void (STDMETHODCALLTYPE *ReleaseWrappedResources)(
        ID3D11On12Device *This,
        ID3D11Resource *const *resources,
        UINT count);

    void (STDMETHODCALLTYPE *AcquireWrappedResources)(
        ID3D11On12Device *This,
        ID3D11Resource *const *resources,
        UINT count);

    END_INTERFACE
} ID3D11On12DeviceVtbl;

interface ID3D11On12Device {
    CONST_VTBL ID3D11On12DeviceVtbl* lpVtbl;
};

#ifdef COBJMACROS
#ifndef WIDL_C_INLINE_WRAPPERS
/*** IUnknown methods ***/
#define ID3D11On12Device_QueryInterface(This,riid,ppvObject) (This)->lpVtbl->QueryInterface(This,riid,ppvObject)
#define ID3D11On12Device_AddRef(This) (This)->lpVtbl->AddRef(This)
#define ID3D11On12Device_Release(This) (This)->lpVtbl->Release(This)
/*** ID3D11On12Device methods ***/
#define ID3D11On12Device_CreateWrappedResource(This,d3d12_resource,flags,input_state,output_state,iid,d3d11_resource) (This)->lpVtbl->CreateWrappedResource(This,d3d12_resource,flags,input_state,output_state,iid,d3d11_resource)
#define ID3D11On12Device_ReleaseWrappedResources(This,resources,count) (This)->lpVtbl->ReleaseWrappedResources(This,resources,count)
#define ID3D11On12Device_AcquireWrappedResources(This,resources,count) (This)->lpVtbl->AcquireWrappedResources(This,resources,count)
#else
/*** IUnknown methods ***/
static FORCEINLINE HRESULT ID3D11On12Device_QueryInterface(ID3D11On12Device* This,REFIID riid,void **ppvObject) {
    return This->lpVtbl->QueryInterface(This,riid,ppvObject);
}
static FORCEINLINE ULONG ID3D11On12Device_AddRef(ID3D11On12Device* This) {
    return This->lpVtbl->AddRef(This);
}
static FORCEINLINE ULONG ID3D11On12Device_Release(ID3D11On12Device* This) {
    return This->lpVtbl->Release(This);
}
/*** ID3D11On12Device methods ***/
static FORCEINLINE HRESULT ID3D11On12Device_CreateWrappedResource(ID3D11On12Device* This,IUnknown *d3d12_resource,const D3D11_RESOURCE_FLAGS *flags,D3D12_RESOURCE_STATES input_state,D3D12_RESOURCE_STATES output_state,REFIID iid,void **d3d11_resource) {
    return This->lpVtbl->CreateWrappedResource(This,d3d12_resource,flags,input_state,output_state,iid,d3d11_resource);
}
static FORCEINLINE void ID3D11On12Device_ReleaseWrappedResources(ID3D11On12Device* This,ID3D11Resource *const *resources,UINT count) {
    This->lpVtbl->ReleaseWrappedResources(This,resources,count);
}
static FORCEINLINE void ID3D11On12Device_AcquireWrappedResources(ID3D11On12Device* This,ID3D11Resource *const *resources,UINT count) {
    This->lpVtbl->AcquireWrappedResources(This,resources,count);
}
#endif
#endif

#endif


#endif  /* __ID3D11On12Device_INTERFACE_DEFINED__ */

HRESULT __stdcall  D3D11On12CreateDevice(IUnknown *device,UINT flags,const D3D_FEATURE_LEVEL *feature_levels,UINT feature_level_count,IUnknown *const *queues,UINT queue_count,UINT node_mask,ID3D11Device **d3d11_device,ID3D11DeviceContext **d3d11_immediate_context,D3D_FEATURE_LEVEL *obtained_feature_level);

typedef HRESULT (__stdcall *PFN_D3D11ON12_CREATE_DEVICE)(IUnknown *device,UINT flags,const D3D_FEATURE_LEVEL *feature_levels,UINT feature_level_count,IUnknown *const *queues,UINT queue_count,UINT node_mask,ID3D11Device **d3d11_device,ID3D11DeviceContext **d3d11_immediate_context,D3D_FEATURE_LEVEL *obtained_feature_level);
/* Begin additional prototypes for all interfaces */


/* End additional prototypes */

#ifdef __cplusplus
}
#endif

#endif /* __d3d11on12_h__ */
