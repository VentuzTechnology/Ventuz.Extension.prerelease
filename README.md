# Ventuz Extensions - API Documentation

_**Documentation Version 0.9 - September 2022**_


Table of Contents

- [Ventuz Extensions - API Documentation](#ventuz-extensions---api-documentation)
  - [Abstract](#abstract)
  - [Using the API](#using-the-api)
  - [Deployment](#deployment)
  - [Architecture](#architecture)
    - [Vx-Tool (vx.exe)](#vx-tool-vxexe)
  - [API: Custom Nodes](#api-custom-nodes)
    - [Constructor and IDisposable](#constructor-and-idisposable)
    - [Persistence](#persistence)
    - [Phases](#phases)
    - [Validation Phase](#validation-phase)
      - [General](#general)
      - [Multiple Outputs](#multiple-outputs)
      - [Value Generators](#value-generators)
      - [Methods and Events](#methods-and-events)
      - [Hierarchy Node Validation](#hierarchy-node-validation)
      - [Layer Validation](#layer-validation)
      - [Resources](#resources)
        - [Meshes](#meshes)
        - [Texture](#texture)
        - [Text](#text)
        - [Material](#material)
        - [Custom Resources](#custom-resources)
    - [Rendering Phase](#rendering-phase)
      - [Push and Pop Materials](#push-and-pop-materials)
      - [Render Mesh](#render-mesh)
      - [Render Text](#render-text)
    - [Design Phase](#design-phase)
      - [Verbs](#verbs)
      - [Icon Index](#icon-index)
      - [Tooltip](#tooltip)
      - [Custom Name](#custom-name)
      - [Used Uris](#used-uris)
    - [Metadata](#metadata)
      - [Node Icon](#node-icon)
      - [Node Toolbox](#node-toolbox)
      - [Node Dynamic Icon](#node-dynamic-icon)
      - [Node Initial Outputs](#node-initial-outputs)
      - [Member Legacy Names](#member-legacy-names)
      - [Member Label](#member-label)
      - [Members Category](#members-category)
      - [Members Tab](#members-tab)
      - [Members Description](#members-description)
      - [Members Default Value](#members-default-value)
      - [Members Favored](#members-favored)
      - [Members Favored](#members-favored-1)
      - [Members String](#members-string)
      - [Members With Alpha](#members-with-alpha)
      - [Members Uri](#members-uri)
      - [Enum Flags](#enum-flags)
  - [Roadmap](#roadmap)

## Abstract

Ventuz Extensions is a new API that is available as of Ventuz version 6.12 - but still in **Alpha** state, so breaking changes are to be expected in newer releases.

Ventuz Extensions (or VX for short) enables programmers to extend the functionality of Ventuz and thus solve tasks that are impossible or difficult to implement with the built-in capabilities.

The main approach is that own nodes can be developed. However, it is also planned that the user interface (GUI) can be extended so that, for example, the import of scene files from other systems is possible.

A Ventuz user can thus extend or adapt the functionality of Ventuz to his or her needs. Third parties can develop generic extensions and offer them to Ventuz users, but can also accept commissioned work to develop specific extensions for Ventuz users. For this purpose, VX will later also provide an interface for license verification. See [Roadmap](#roadmap)

## Using the API

A Ventuz extension is basically always a .Net assembly that must meet certain requirements:
* .Net Class Library
* .Net Framework 4.8
* a reference to the current version of `Ventuz.Extension.dll` (found in the installation folder of Ventuz)
* x64 processor architecture
* recommended language: C# (others have not been tested)
* assembly must have the file extension `.vx.dll`

During the development of a VX code (assembly) it is recommended to configure the output folder as described in [Deployment](#deployment) and to set `Ventuz.exe` or `VentuzPresenter.exe` as *host process* in the Debug Configuration ("Start external program"). Starting the debugger then starts Ventuz (Designer or Runtime) as a host process and debugging runs completely in Microsoft Visual Studio or Visual Studio Code in a very pleasant way. 

Vx code is currently loaded once when the Ventuz process is started. Changes to the code therefore require a complete restart of Ventuz or the debugger. Edit and Continue is possible, however! Unfortunately, .net 4.8 does not allow reloading executable code without a huge performance hit. This might change once Ventuz will run on .net 6. See [Roadmap](#roadmap)

## Deployment

This assembly can load other assemblies or DLL's or perform p/invoke calls. The Vx assembly will be executed in the same process as Ventuz and thus have full system access! Thus, the execution of Vx code represents a certain security vulnerability! However, the current version of Vx does not take this into account. So a user who wants to use third party Vx code must trust the source of the code. These security holes will be closed in the next versions of Ventuz. See also [Roadmap](#roadmap).

Vx Assemblies will later use NuGet technology to install, manage, update and uninstall installed extensions. Version and dependency management will also be handled by NuGet. See [Roadmap](#roadmap)

In the current version, a Vx code must be present in a specific location to be detected by Ventuz. Currently this is the path:

```
C:\Users\<username>\Documents\Ventuz6\Vx
```

This path can be changed, however, by editing the file in the Ventuz installation directory

```
VentuzEnvironment.ini
```
and the entry
```
# Folder to search for Ventuz Extensions (VX)
;VX_MANUAL_CODE_LOCATION=%HOMEDRIVE%%HOMEPATH%\Documents\%VENTUZVER%\Vx
```
and thus redirect `VX_MANUAL_CODE_LOCATION` to another folder on your system (you have to uncomment the line by removing the semi-colon prefix). This is highly recommened for Vx-Developers, because you can point the Vx folder to your code folders and make sure to have the correct access right at this location. Final installations can either use the default path or redirect to a special location for easier management.

* Each individual Vx code (or assembly) must reside in its own subfolder, ideally named the same as the actual assembly. 
* The Vx assembly itself must end with the file extension `.vx.dll` to be recognized by Ventuz.
* Only the first found assembly that matches this naming pattern within a folder will be loaded. So make sure that there are not several files with this extension in one subfolder.

Example:
```
MyFirstVxCode\MyFirstVxCode.vx.dll
```

Other files needed by the Vx code should also be located in this folder. This structure is considered as a preparation to the NuGet integration (see [Roadmap](#roadmap)). Dependencies to other assembliess are currently not managed by Ventuz. NuGet will take care of this later.

Now, if a Vx code is needed on another system, simply copy the entire folder into the Vx directory of the target system. Done!

## Architecture

Vx code is based on the definition of classes, types and attributes. Ventuz will analyze all type definitions when loading a Vx assembly and provide them accordingly. Types that contain errors will not be loaded!

### Vx-Tool (vx.exe)

Since Ventuz 6.12, a prototype of the VX tool is located in the Ventuz installation directory:
```
Vx.exe
Ventuz.Extension.dll
Ventuz.Extension.Analytics.dll
```
This tool is currently very primitive, but is very useful to have the Vx code analysis done outside of Ventuz. So a developer could use the Vx tool as a post-build step to quickly detect errors in the Vx code. If you want to use `vx.exe` outside the Ventuz installation directory, you have to copy the two assemblies mentioned above as well, because `vx.exe` is directly dependent on them. Use `vx.exe` with the Vx-Assembly as parameter like this:
```
Vx.exe MyFirstVxCode.vx.dll
```
You will see an output on the screen that contains some analysis information, warnings and errors. Please do not rely to this output for now, as it will change in later releases.

## API: Custom Nodes

Vx allows you to develop your own nodes. The type of node defines its functionality and possibilities. However, these nodes have some things in common. More about this later. The following node types are currently available:

| VxType         | base class        |
| -------------- | ----------------- |
| Content Node   | `VxContentNode`   |
| Hierarchy Node | `VxHierarchyNode` |
| Layers         | `VxLayerNode`     |

To define your own node, you just have to declare a `public class` that derives from the corresponding `base class`. This class must not be abstract! 

Here an Example to declare a simple `Content Node`. This implementation defines a Content Node that does nothing, but it will appear in the Ventuz Toolbox with its class name `MyEmptyNode` under the `Vx` tab/category, because it doesn't define any [Metadata](#metadata-for-nodes) yet.

```c#
public class MyEmptyNode : VxContentNode
{
}
```

### Constructor and IDisposable

Vx Nodes can implement
* a parameterless public constructor which is called whenever this node is created
* `IDisposable` is called when the node is deleted or the scene containing the instance gets deleted.

It is not recommened to use a constructor and implement `IDisposable`, because this let us assume that you want to do anything with external resources, threads or other *unmanaged* elements. Instead consider the use of [Custom Resources](#custom-resources)!

### Persistence

A Vx node can implement `IPersistence` to save and load additional data with the node. 
```c#
public interface IPersistence
{
    void Serialize(VxPersistentData dataToFill);
    void Deserialize(VxPersistentData data);
}
```

The class `VxPersistentData` is used to serialize/deserialize simple primitive data as Dictionary (Key-Value-Pairs). It only supports the following primitive types. For more complex data you should consider using JSON text or base64 encoded binary data.

```c#
    bool,
    byte, sbyte,
    short, ushort,
    int, uint,
    long, ulong,
    float, double, decimal,
    string,
```

### Phases

Ventuz is a real-time graphics system designed for speed, performance and perfect balance between CPU and GPU. Therefore, it is important that certain operations are only executed in certain phases of the rendering process in order to not disturb the balance! It is also important that the right thread executes the right code at the right time. Therefore it is strongly advised not to use custom threads or `async` tasks if you are not sure whether this is allowed at a given point!

Three phases are currently usable in Vx:

**[Validation Phase](#validation-phase)**\
Here information about the input properties are processed and results are sent to the output properties and thus to dependent nodes. Events are also received and sent in the validation phase. Resources are also generated in the validation phase.

**[Rendering Phase](#rendering-phase)**\
In the rendering phase, elements are sent to the renderer, but not rendered directly. This may happen several times depending on the view, stereoscopy and other things. In this phase, no further write accesses to the elements of the validation can take place.

**[Design Phase](#design-phase)**\
The Design phase is "before" Validation and Rendering. Structural changes to the scene or UI elements can be performed here. Actions in the Designer are disruptive and can affect the performance of Validation and Rendering in such a way that a constant and optimal frame rate is no longer guaranteed. The Ventuz Designer GUI works exclusively in the Design phase.

### Validation Phase

#### General

Nodes can receive data from other nodes in the validation phase, distribute it and generate new data. For example: results from calculations or new resources (textures, geometries, etc.).

A node is only validated - called in the validation phase - if one or more of its input properties have changed. More precisely, the sending node determines whether a value has changed by informing its dependent nodes. The actual value does not necessarily have to have changed.

Here is an example of a very simple validation. A validation function is defined here that declares two input properties (`A` and `B`) of type `float` and the output property `Sum` again of type `float`.
Validation functions must start with the prefix `Validate...`. If this validation returns a value, then this value must be appended directly to the prefix with its name (here `Sum`).

```c#
public class MySimpleSum : VxContentNode
{
    public float ValidateSum(float A, float B)
        => A + B;
}
```

A node can (and should) define multiple validation functions. The reason is simple: If certain input properties change, then it is not always necessary to calculate the entire validation again. So it makes sense to consider which inputs lead to which results. Here is a (primitive) example. The upper sum is to be multiplied by a factor. If only the factor changes, the sum of `A` and `B` does not need to be calculated again. This is an abstract example and is only meant to illustrate the use of split validation:

```c#
public class MySimpleSumFact : VxContentNode
{
    public float ValidateSum(float A, float B)
        => A + B;

    public float ValidateResult(float Sum, float Factor)
        => Sum * Factor;
}
```

Please note that the name `Sum` and the type of it `float` must match at all places where they are used to be considered as the same.

The Vx system analyzes all validation functions found, their input and output properties and creates a dependency tree of them. In this way, Ventuz knows exactly under which changes in the properties the functions must be called: If only `Factor` changes and `Sum` has already been calculated, `ValidateSum` will not be called anymore.

This node will now have the following properties:
* Input: `A`, `B` und `Factor`
* Output: `Sum` und `Result`

What if the intermediate result `Sum` should not be visible to the user at all? In this case we use an underscore as prefix for the name of the value: `_Sum`.

```c#
public class MySimpleSumFact2 : VxContentNode
{
    public float Validate_Sum(float A, float B)
        => A + B;

    public float ValidateResult(float _Sum, float Factor)
        => _Sum * Factor;
}
```

By convention, names with an underscore prefix are treated as *Intermediate* and are not visible to the node user and generally not processed further by Ventuz. Using *Intermediate properties* has two important advantages:
* you do not have to define any data fields in your class. Ventuz takes care of storing the actual values.
* if there are no fields defined to store these values, you have to define them in the parameter scope of the Validation function. This prevents you from accessing them wrongly, because the compiler will help you here.
*Intermediate properties* can be of any type - as well custom types, while visible properties must be handled by Ventuz, bindings and properties are therefore restricted to the following types:

**Supported Ventuz Property Types**

```c#
// primitives
bool, int
float, double

// enumerations of type Int32
System.Enum

// math
Ventuz.Vx.Matrix44, Ventuz.Vx.Matrix44A
Ventuz.Vx.Color

// immutable objects
string, 
System.Uri

// arrays
bool[], int[], float[], double[], string[]

// math arrays
Ventuz.Vx.Matrix44[], Ventuz.Vx.Matrix44A[]

// resources
Ventuz.Extension.ITexture
Ventuz.Extension.IMesh
Ventuz.Extension.IText
Ventuz.Extension.IMaterial
Ventuz.Extension.ICustomResource
```
#### Multiple Outputs

What is if my Validation function produces more than just a single value? There are several option to output multiple values:
1. Use *named tuples* as return value (the name of the `Validation...` function is not used anymore but must specify any dummy name to follow the convention)
2. Use *out* parameters. In this case the Validation Function can return void, but the name of the `Validation...` must specify any dummy name to follow the convention
3. A mix of 1 and 2

Examples:
```c#
// return a named tuple
public (float X, float Y) Validate1(float X1, float Y1, float X2, float Y2)
    => (X1 + X2, Y1 + Y2);
```
```c#
// use out parameters
public () Validate1(float X1, float Y1, float X2, float Y2, out float X, out float Y)
{
    X = X1 + X2;
    Y = Y1 + Y2;
}
```
```c#
// return a named tuple and out parameters
public (float X, float Y) Validate1(float X1, float Y1, float X2, float Y2, out float TotalSum)
{ 
    TotalSum = X1 + X2 + Y1 + Y2
    return (X1 + X2, Y1 + Y2);
}
```

Please note that Input Properties cannot be declared as Tuples!

#### Value Generators

If a node needs to be validated every frame, regardless if an Input Property has changed or not or it even does not come with any inputs at all, the function is called a *Generate Function* and follows the naming rule `Generate...`. 

This Node example generates a random number every frame even if its Input Property `Range` doesn't change:

```c#
public class MyRandomGenerator : VxContentNode
{
    Random RND = new Random();

    public float GenerateRandom(float Range) 
        => (float)(RND.NextDouble() * Range);
}
```
#### Methods and Events

The validation phase also allows to receive events (`Methods`) or send events (`Events`)

In order to define event receiver methods just declare a function that follows these rules:
* Visibility is `public`
* Function name must start with `Method...' 
* return type is `void`
* optionally add parameters to receive input, output or intermediate properties

If the Method was called by an event that carried an argument, you can retrieve its value by reading the MethodArg property. It returns null, if no argument was attached.

```c#
// define a method named 'Nudge' without argument
public void MethodNudge() 
{
    int arg = MethodArg.GetValueOrDefault(0);
    // do something with int32 argument
}

```

If you want to emit events, simply declare a field in your node of type `Ventuz.Extension.Event`. This field is completely managed by Ventuz and is never *null*. Only use this field to emit the event. The delegate `Event` has two optional parameters:
* **int arg**: optional int32 argument to be attached to the event (default 0)
* **int delay**: number of frames this event is delayed before received by any other node (default 0)

```c#
// this node fires an event if the input 'Fire' turns to true
public class MyBang : VxContentNode
{
    // save the previous value to make sure the event is only fired if we have a real change from false/undefined to true
    bool? prevFire;

    // declares an event named 'Bang'
    Event Bang;

    public void Validate1(bool Fire)
    {
        if( prevFire != Fire )
        {
            prevFire = Fire;

            if( Fire )
                Bang();
        }
    }
}
```
#### Hierarchy Node Validation

Custom Vx Nodes, which are derived from VxHierarchyNode, have another API within the validation phase to influence the outputs of the node. With this method individual settings can be made at each output:
* Visibility (Blocked)
* Alpha (Transparency)
* Axis (World)
* Order (order in which the outputs are rendered)

```c#
bool SetOutput(int index, bool? blocked = null, float? alpha = null, in Matrix44A? axis = null, int? order = null);
```

#### Layer Validation

Like the VxHierarchyNode can, custom Vx Layers modify their sub content. Layers do not have outputs like Hierarchy Node have, but child layers.
The Layer API provides a method to modify settings how the children are processed and rendered:
* Visibility (Blocked)
* Opacity (Transparency, inverse alpha)
* Order (order in which the child layers are rendered)
  
```c#
bool SetChildOptions(int index, bool? blocked = null, float? opacity = null, int? order = null);
```

#### Resources

Resources can also be created during the validation phase. This process is somewhat more complex, as Ventuz has an internal *Resource Management* that prevents duplicate generation and optimally manages memory usage. To fulfill this purpose, each resource must be able to identify itself uniquely. This is done via its so-called *ParameterSet* and its *ResourceGenerator*. In Vx these two factors are combined to keep it as simple as possible.

Each node has access to resource generation functions. In the current version of Vx the following resources are available:
* **Mesh**
* **Texture**
* **Text** (layout Text, like BlockText does)
* **Material** (not yet generatable, but usable as input and output property)
* **Custom Resources** (especially for VX, see [Custom Resources](#custom-resources))

##### Meshes

The following example validates a `Size` parameter into a simple rectangle geometry. It uses the function `CreateMesh` to validate the Output Property `Geo`. The name of the Output property is specified at the call to `CreateMesh` to let Ventuz know with property storage to use. Since this example has only a single `IMesh` output defined, this name could be *null* - Ventuz would use the first and only IMesh property found. It is recommended to always specify the name to the `Create...` functions.

An additional parameter of inner type `PS` is given. This type must be a `struct` and must implement `IMeshResourceParameter`. This struct is either used to identify the object and - if necessary - generate the resource. The identification is simply done by comparing the *ParameterSets* from existing resources with the one provided to the `Create...` call. The equality check is done by calling `object.Equals()`, so it may make sense to override implement the methods `Equals` and `GetHashcode` for such *ParameterSets* to improve performance as stuct compares in .Net are very slow in the default implementation (see e.g. https://devblogs.microsoft.com/premier-developer/performance-implications-of-default-struct-equality-in-c/).

You can also override the `ToString` method to give some useful information in the Ventuz Resource Statistics view.

If the Ventuz Resource Manager cannot find an existing resource that equals the requested one, the implementation of `IMeshResourceParameter` is called to generate the resource (here the mesh).

The current API for creating meshes is quite simple. Create an array of vertices (there are several types define) and an index buffer (16bit ushort or 32bit uint) and call `CreateTriangleList`. Other mesh types like TriangleFan are not supported yet. See [Roadmap](#roadmap) 

Finally the call to `CreateMesh` will return an existing mesh that matches the *ParameterSet* or the newly created resource.

The resulting `IMesh` can now be returned as Output Property to be used by other nodes (for example the *Geometry Renderer* node) or it is used internally during the [Rendering Phase](#rendering-phase) of the same node. In this case the `IMesh` must be stored locally in a class field or the name must follow the naming rules for intermediate properties (rename `ValidateGeo` to `Validate_Geo` and return void).

```c#
public class MySimpleRect : VxContentNode
{
    public IMesh ValidateGeo(float Size)
        => CreateMesh("Geo", new PS() { Size = Size });

    public struct PS : IMeshResourceParameter
    {
        public float Size;

        public IMesh GenerateMesh(IMeshFactory meshFactory)
        {
            var v = new VertexPN[4];

            v[0].px = -Size;
            v[0].py = Size;
            v[0].nz = -1.0f;

            v[1].px = -Size;
            v[1].py = -Size;
            v[1].nz = -1.0f;

            v[2].px = Size;
            v[2].py = Size;
            v[2].nz = -1.0f;

            v[3].px = Size;
            v[3].py = -Size;
            v[3].nz = -1.0f;

            var i = new ushort[6];

            i[0] = 0;
            i[1] = 2;
            i[2] = 1;

            i[3] = 1;
            i[4] = 2;
            i[5] = 3;

            return meshFactory.CreateTriangleList(v, i);
        }
    }
}
```

##### Texture

Ventuz Extensions also allows you to generate textures. The current API is still relatively simple, but already offers sufficient possibilities. Please also read the section [Meshes](#meshes) to get familiar with resource generation in general.

This API is currently not suitable for generating high-performance textures (e.g. live video). However, procedural texture generators, file loaders or even custom rasterizers could be implemented.

The texture generation follows the same rules as described in [Meshes](#meshes), except that in this case the `ITextureResourceParameter` must be implemented. The actual API call is then `CreateTexture`.  

**Warning:** This API uses, among other things, direct pointers to native memory. Incorrect access to this memory can lead to *memory violations* and may crash the entire process. Careful handling is therefore advised!

This API is currently still relatively slow and not optimized. The Vx developer keeps his own memory with the pixel data (here the bitmap), Ventuz will then copy this data directly into the CPU/GPU memory. Afterwards the memory can be freed again on the Vx side (here 'UnlockBits').

The resulting resource is represented by `ITexture`. This type can be used as input and output property to be bound to **Texture Loader** or **Material** Nodes. 

```c#
public class MyTextureGenerator : VxContentNode
{
    public enum MyColors
    {
        Red, Green, Blue
    }

    public struct PS : ITextureResourceParameter
    {
        public string Text;
        public MyColors Color;

        public ITexture GenerateTexture(ITextureFactory textureFactory)
        {

            using (var bm = new Bitmap(100, 100, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (var g = Graphics.FromImage(bm))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                    g.Clear(System.Drawing.Color.Empty);
                    var brush = Brushes.White;
                    switch (Color)
                    {
                        case MyColors.Red:
                            brush = Brushes.Red;
                            break;
                        case MyColors.Green:
                            brush = Brushes.Green;
                            break;
                        case MyColors.Blue:
                            brush = Brushes.Blue;
                            break;
                    }
                    g.FillEllipse(brush, new Rectangle(0, 0, bm.Width - 1, bm.Height - 1));
                }
                var bits = bm.LockBits(new Rectangle(0, 0, 100, 100), System.Drawing.Imaging.ImageLockMode.ReadOnly, bm.PixelFormat);
                try
                {

                    return textureFactory.CreateTexture((uint)bm.Width, (uint)bm.Height
                        , true, bits.Scan0, format: TextureFormat.RGBA_UN8);
                }
                finally
                {
                    bm.UnlockBits(bits);
                }
            }
        }
    }

    public ITexture ValidateImage1(string Text1, COLOR Color1)
        => CreateTexture("Image1", new PS() { Text = Text1, Color = Color1 });

    public ITexture ValidateImage2(string Text2, COLOR Color2)
        => CreateTexture("Image2", new PS() { Text = Text2, Color = Color2 });
}
```

##### Text

Text in Ventuz is separated in two different entities:
* `IText` (text layout, like a BlockText Node produces)
* `ITextRenderObject` (resource that can be rendered)

A Text resource does not need the complex way of `ParametSets`, because the parameters used to layout a text are always the same:
* The text itself
* Typeface (the font to use)
* Layout options (like known from the BlockText node)

This simple example layouts a text by using the `Code` typeface preset configured in the project settings. It has an Input Property `Number` which is simply laid out as a formatted string - without any special layouting options. The `Typeface` parameter is a *string* that can either be 
* *null*, for default font,
* a string with the `$` prefix to lookup a typeface preset configured in the *Ventuz Project*, or
* an encoded typeface. The formatting of this string is basically the Family Name and Style of the font. However, probably the easiest way is to use a BlockText node in Ventuz and configure a typeface (*BlockText->Font->Edit...*) and copy the string from the `Font` property.

The resulting `IText` object can currently be used as input and output property. It is also required to generate `ITextRenderObject` resource if you like to render the text - otherwise the normal `Text 2D` and `Text 3D` node can receive an `IText` and render it.

```c#
public class MyBlockText : VxContentNode
{
    public IText ValidateText(int Number)
        => CreateText("Text", Number.ToString(), new TextLayoutOptions(), "$Code");
}
```

This example also uses the API `CreateTextRenderObject2D` which uses an `IText` object to generate a resource that can be rendered by Vx-Code. Currently it is not possible to create 3D resource for text rendering. See [Roadmap](#roadmap)

The method `CreateTextRenderObject2D` has an optional `style` parameter. The following options are available:
* *null*, for a default style
* a string with the `$` prefix to lookup a style preset configured in the *Ventuz Project*, or
* an encoded style. The formatting of this string is quite complex and should not be done by hand. However, probably the easiest way is to use a `2D Text` node in Ventuz and configure the style (*2D Text->Style->Edit...*) and copy the string from the `Style` property.

See also [Rendering Phase](#rendering-phase) for information about the `RenderText` call.

```c#
public class MyTextRenderer : VxHierarchyNode
{
    IText text1, text2;
    ITextRenderObject textRender1, textRender2;

    public void ValidateText(string Text1, string Text2)
    {
        CreateText(ref text1, Text1, new TextLayoutOptions(), "$Code");
        CreateText(ref text2, Text2, new TextLayoutOptions(), "Arial, Bold, Italic, Condensed";
        CreateTextRenderObject2D(ref textRender1, text1, "$Neon Glow");
        CreateTextRenderObject2D(ref textRender2, text2, 
            "2D Font,  (0;0) Textured, , Ballon (255;255;0;255) 0.3 0 0.3 (0;0;0) None, Outline (255;0;255;0) 0.2 0 0 (0;0;0) None");
    }

    public void RenderText()
    {
        for ( int i = 0; i < 10; i++ )
        {
            RenderText(textRender1, Vx.Matrix44A.Translation(-5, i, 0));
        }

        for ( int i = 0; i < 10; i++ )
        {
            RenderText(textRender2, Vx.Matrix44A.Translation(0, i, 0));
        }
    }
}
```

##### Material

Materials are represented by the `IMaterial` type and can be used as Input and Output Properties. 

`IMaterial` inputs can be used to give a node certain material(s) used during the [Rendering Phase](#rendering-phase), or it could output a switched selection of it (a material switcher).

The current version of Vx does not provide an API to create your own materials. See [Roadmap](#roadmap). 

##### Custom Resources

Another resource type that is only known in the Vx-context if the `Custom Resource' which can be used to interchange information from one Vx-Node to another and the resource management should be used to avoid multiple resources of the same content. 

A good example could by a native data connection to a proprietary server. One Vx Node represents this connection with its parameters (connection string) as a *custom resource*. Another Vx-Node knows the type of this *custom resource* and uses it to gather data from the connection (the same technique is used by the original Database nodes from Ventuz, also the Serial Text node - COM does the same).

The example below defines two content nodes `MyConnectionCreator` which is responsible to create the connection and `MyConnectionUser` what is one user of this connection. The class 'MyConnectionResource' represents the actual resource object - the *connection* - and must implement `ICustomResource`. The *ParameterSet* must implement `ICustomResourceParameter<MyConnectionResource>`. That's it!

If you are not aware of *ParameterSets* please read section [Meshes](#meshes)

```c#
public class MyConnectionCreator : VxContentNode
{
    public struct PS : ICustomResourceParameter<MyConnectionResource>
    {
        public int Parameters;

        public MyConnectionResource GenerateResource()
            => new MyConnection(Parameters); // give connection parameters
    }

    public MyConnectionResource ValidateConnection( int SomeInput )
        => CreateCustomData("Connection", new PS() { Parameters = SomeInput });
}

public class MyConnectionUser : VxContentNode
{
    MyConnectionResource Connection;

    public void Validate1(MyConnectionResource Connection)
    {
        // just store my connection
        this.Connection = Connection;
    }
        
    public void MethodTrigger(int TriggerValue)
        => this.Connection?.Trigger(TriggerValue);
}

// the actual resource object
public class MyConnectionResource : ICustomResource
{
    public MyConnectionResource(int parameters)
    {
        // implement connection
    }    

    public void Trigger(int triggerValue)
    {
        // trigger something on my connection
    }
   
    public override string ToString()
        => "return some useful text to hint the user what this is";
}
```

### Rendering Phase

The API for rendering is still relatively simple, but already quite powerful! Many tasks can already be solved with it.
* [Push and Pop Materials](#push-and-pop-materials) (`IMaterial`)
* [Render Mesh](#render-mesh) (`IMesh`)
* [Render Text](#render-text) (`ITextRenderObject`)


Currently only the Hierarchy Node (derived from `VxHierarchyNode`) can use the rendering phase. For this, one or more `Render...` functions must be defined. These follow the same naming conventions as `Validate...`, `Method...` and `Generate...`, but the name does not matter. The name is only used to order the calls by sorting them alphabetically.

This Example renders one given `Geometry` two time with two different materials `Material1` and `Material2`.

Please note that `Render...` functions can also have their Input Properties declared as parameters, so there is no need to store them in member fields of your node class.

```c#
public class MyRenderer : VxHierarchyNode
{
    public void Render1(IMesh Geometry, IMaterial Material1)
    {
        if ( Geometry != null )
        {
            PushMaterial(Material1);
            RenderMesh(Geometry);
            PopMaterial();
        }
    }

    public void Render2(IMesh Geometry, IMaterial Material2)
    {
        if ( Geometry != null )
        {
            PushMaterial(Material2);
            RenderMesh(Geometry);
            PopMaterial();
        }
    }
}
```

#### Push and Pop Materials

The calls `PushMaterial` and `PopMaterial` allows you to set a material (received via input property of type `IMaterial`) to/from the material stack. Materials are always cascaded, so materials already on the stack are also applied!

Any subsequent render will use the entirely pushed material stack.

If a material is left on stack (`PopMaterial` was not called), the next `Render...` function will inherit this material as well as other nodes connected to any of the outputs of a `VxHierarchyNode`.

#### Render Mesh

The `RenderMesh` API function can be used during the Render Phase of HierarchyNodes. It received the mesh to be rendered and an optional array of instances. If this array is omitted the mesh will be rendered once with defaults. A null array renders nothing. The `RenderMeshDetails` have several options how and how many times a mesh should be rendered. The length of the array simply decides the number of instances to be rendered.

* **AxisMode**
  * **Identity** `Axis` is not used, mesh will be rendered at current world transformation
  * **Relative** `Axis` will be relative to the current world transformation
  * **Absolute** `Axis` will override the current world transformation
  * **DoNotRender** the mesh is not rendered at all
* **Axis** (world axis to position the mesh)
* **Subset** if the mesh has subsets (`IMesh.SubsetCount > 0`), the index to render. The value of `null` or any negative number renders all subsets
* **Frame** if the mesh has frames (`IMesh.FrameCount > 0`), this intra-frame index to render. 

```c#
void RenderMesh(IMesh mesh, RenderMeshDetails[] details = null);
```

#### Render Text

The `RenderText` API function can be used during the Render Phase of HierarchyNodes. It is able to render a previously create resource of type `ITextRenderObject` with an optional world axis. See [Text Resource](#text) section about how to create text render objects.


```c#
void RenderText(ITextRenderObject textRenderObject, in Vx.Matrix44A? axis = null);
```

### Design Phase 

The Design Phase is actually *between* the rendered frames, before *Validation* and *Rendering*. In this phase, the scene graph (nodes and others) could be modified. This is what Ventuz Designer does. The current Design API is quite simple and currently only for nodes and their required GUI features, but it is planned to give full access to the scene tree, all nodes, internal and external Vx-nodes in later versions of Vx. See [Roadmap](#roadmap)

#### Verbs 

Every Vx-Node can implement `verbs`. A verb is a GUI-action, displayed as a button at the bottom of the Property and Layer Editor. If the user presses such a verb button the dedicated method in your Vx-code is called.

The verb functions are declared as method of your node class, while the usual naming rules take place again; a method named `VerbDoSomething` defines a verb with the name `DoSomething`.

The Property and Layer Editor calls the Verb functions several times to probe if the verb is currently available. In this case the `probe` argument is `true` and the method implementation should not perform any action but return the state of the verb. If the verb was clicked by the user the `probe` parameters is `false` and the implementation should do its job. The returned VerbState is ignored after execution.

```c#
public VerbState VerbDoSomething(bool probe)
{
    if( probe )
    {
        // check state
        return VerbState.Visible; // or VisibleChecked, Disabled, Invisible
    }

    // perform the action here
    return VerbState.Disabled; // return value ignored if 'probe' is false
}
```

#### Icon Index

Every Vx Node can define one or more icons to be displayed in Ventuz Designer (see [Metadata](#metadata)), if the node implements the interface `IIconIndex` to tell Ventuz which icon index to use.

***Please note that this interface is called every time the GUI is drawn! So keep the implementation as fast as possible!***

```c#
public interface IIconIndex
{
    int IconIndex { get; }
}
```

#### Tooltip

If a Vx Node implements the interface `ITooltip` the user can received additional information about the state of a node when hovering with the mouse over it. 

***Please note that this interface is called every time the GUI wants to display the tooltip! So keep the implementation as fast as possible!***

```c#
public interface ITooltip
{
    string Tooltip { get; }
}
```

#### Custom Name

Any node in Ventuz can have a name which is changeable by the user. If a Vx Node implements the interface `IName` the user cannot edit or change the name anymore, instead a static name is displayed in the GUI. This is very useful if simple nodes must display a state as short text and a custom name is not required.

***Please note that this interface is called every time the GUI is drawn! So keep the implementation as fast as possible! Consider a caching for the returned name if it does not change.***

``` c#
public interface IName
{
    string StaticName { get; }
}
```

#### Used Uris

Vx Nodes may reference files represented by `Uri`. By implementing the interface `IUsedUris`, the node has the ability to report all used Uri reference to Ventuz if the scenes containing the node is exported as VPR (presentation), VZA (scene archive) or VPA (project archive)

**This interface is only called during export operations and does not have to be real-time capable.**

```c#
public interface IUsedUris
{
    Uri[] GetUsedUris();
}
```

### Metadata

All code example shown above are not using any metadata to make names and other GUI information comfortable to human readers. Names are limited to the naming convention of C# (.net) and cannot contain spaces or special characters. Also icons and other descriptive information are kept very simple if you do not annotate your code with metadata. To do so, Vx uses `Attributes` to decorate your class, methods, fields, etc.

There are different groups of metadata attributes available:
* for nodes (classes)
* for single members
* for multiple members
* for enum fields/values

Attributes for members are mostly defined at class definition, because Vx-members definitions appear at multiple locations in the code (e.g. multiple validation functions). To get the best overview about metadata they are mostly defined at the top of the class:

```c#

[VxToolBox("String Splitter VX", "Glare", "Split", "This node splits a text string into text fragments which can be accessed by their index.", false)]
[VxIcon("NodeIcons.Logic.Expressions")]
[VxCategory(0, "Text", false, "Input", "Current", "RemoveEmptyItems")]
[VxCategory(1, "Separator", false, "Custom", "Lines", "Tabs")]
[VxCategory(2, "Output", false, "Items", "Count", "IsNullOrWhitespace", "Item")]
[VxDefaultValue("", "Input", "Custom")]
[VxDefaultValue(0, "Current")]
[VxDefaultValue(false, "RemoveEmptyItems", "Tabs")]
[VxDefaultValue(true, "Lines")]
public class VX_TEXT_SPLITTER : VxContentNode
{
    ....

```

#### Node Icon

`VxIconAttribute` is used at class level and assigns an icon resource to be displayed as node icon. 
* **Name** The full qualified name of the embedded resource within your vx-assembly
* **Index** Index of the icon. See below...

Add an image resource to your project and select `Embedded Resource` for this item. Please note that the `Default Namespace` and any subfolders are included in the full qualified name of the resource! 

* A project with default namespace `MyCompany.MyProject`
* and a sub folder `Icons`
* and a resource with the name `MyRandomGenerator.png`

...will result in a full qualified name of 

`
"MyCompany.MyProject.Icons.MyRandomGenerator.png"
`

Icons for Nodes must be
* 32 x 32 pixels
* alpha channel is supported
* standard, simple image formats: PNG, JPG, BMP
* simple SVG is also supported, while signal colors `Magenta (#FFFF00FF)` and `Cyan (#FF00FFFF)` are replaced by the Skin-Color (dark and light) from Ventuz. Please note that not all SVG feature are supported. Text for example should always be converted into curves, because the used font may not be available on the target system. As well pixel based images for fill-patters may be rasterized wrong. The Ventuz SVG Node uses the same rasterizer - a good way to test if your SVG is compatible.

The optional `Index` allows you to assign multiple icons for a single class. Vx Nodes can decide which icon to use by implementing `IIconIndex`. See [Icon Index](#icon-index)

#### Node Toolbox
`VxToolBoxAttribute` can set some information how a Vx Node appears in the Toolbox:
* **Name** overrides the class name which is used by default.
* **Category** the Toolbox-Tab or Category the node appears in
* **DefaultName** the name of the node if an instance is created. If the name contains a "*" it will be replaced by an unique count. A DefaultName of `"Node*"` would procude names like this `"Node1"`, `"Node2"`, `"Node3"`, ...
* **Description** A short description that is displayed in the Toolbox when hovering or in the dynamic help bar (bottom of Designer, Shift-F1)
* **Small** Only applicable for Content Nodes. New instances are displayed only half the size and name is not rendered.

#### Node Dynamic Icon
`VxDynamicIconAttribute` can be used to specify the name of a `IText`, a `IMesh` or a `ITexture` input/output property. Ventuz will render a preview icon of this resource to be displayed.
* **Member** the property name of the resource

#### Node Initial Outputs
`VxInitialOutputsAttribute` can be used on Node of type `VxHierarchyNode` and defines the number of outputs this node should initially have and how they are named. If this attribute is not present, a single output is created. An empty array will result in a node without any outputs.
* **InitialOutputNames** array of names to define number and names of output a Vx Hierarchy Node initially has.

#### Member Legacy Names
`VxLegacyNamesAttribute` specifies one or more legacy names for a single member. If the Vx developer decides to rename a property, this attribute would let Ventuz know about its previous name(s). This is very important to keep scenes compatible when saved with an older version of the Vx code and the new version wants to read (import) that older scene!
* **Member** the name of the member this attribute belongs to
* **LegacyNames** list of legacy names.

***Legacy names are only used if a loaded property could not be found by its name. Therefore a cross-rename is not possible!***

#### Member Label
`VxLabelAttribute` is used to rename the technical name into a human readable text. A name like 'ClearContent' can be renamed in 'Clear Content`
* **Member** the name of the member this attribute belongs to
* **Label** Text to display in GUI. (property editor)

#### Members Category
`VxCategoryAttribute` defines the category (or group) of members. The Property Editor displays a folder for all members in the same Category. If this attribute is omitted the default category `Misc` is used.
* **Members** the name(s) of the member(s) this attribute belongs to
* **Category** the display name of the category
* **DefaultCollapsed** indicates if this category is collapsed or expanded by default.

#### Members Tab
'VxTabAttribute' is used to group properties under a Tab within the property editor.
* **Members** the name(s) of the member(s) this attribute belongs to
* **Tab** the display name of the tab

#### Members Description
`VxDescriptionAttribute` can add a descriptive text for one or multiple members. This description is displayed when hovering over the member in the Property Editor..
* **Members** the name(s) of the member(s) this attribute belongs to
* **Description** the description text. Can be multi-line by using new-line characters `"\n"`.

#### Members Default Value
`VxDefaultValueAttribute` assign default values to one or more input properties. The property editor can then reset-to-default
* **Members** the name(s) of the member(s) this attribute belongs to
* **Value** the value used as default.

***It is mandatory that the specified literal default value is of the same type as the members. For example float properties must specify their default value with the f-modifier in C#: `1.0f`. A special attribute constructor with parameters `type` and `value` can be used to call the available TypeConverters for the given `type` to convert the `value` into the actual type of the property.***

#### Members Favored
`VxFavoredAttribute` defines if the properties are 'favored'. If the Property Editor is configured to display only 'favored' properties when the node is selected in Hierarchy Editor (see option of Property Editor), see Property Editor gets less populated and improved readability.
* **Members** the name(s) of the member(s) this attribute belongs to

#### Members Favored
`VxNumericAttribute` applies only to numeric properties (`float`, `int`, `double`). It defines certain options how the numeric value is displayed and edited in the Property Editor.
* **Members** the name(s) of the member(s) this attribute belongs to
* **MinValue** if specified, the minimum value 
* **MaxValue** if specified, the maximum value 
* **StepValue** if specified, the edit speed when dragged&moved by mouse
* **Unit** a string displayed after the numeric value (e.g. "px" for pixels)
* **Mode** if min and max values are specified the Property Editor can render additional graphical hint (like a bar from 0% to 100%) 
  
***The literal type of the given values must match the type of the addressed properties. For example `float` properties must use literals like this `"1000.0f"`***

#### Members String
`VxStringAttribute` decorates string input properties and tells Ventuz how to use the Text Editor.
* **Members** the name(s) of the member(s) this attribute belongs to
* **Type** type of the string/text (SingleLine, MultiLine, CSharp, XML) 
* **LiveUpdate** if true, the nodes gets updated/validated on every change made in the Text Editor. Otherwise the update is done if the Text Editor loses focus.

#### Members With Alpha
'VxWithAlphaAttribute' instructs the Property Editor to display also the ***A*** (alpha) value for a color. This attribute only applies to color properties.
* **Members** the name(s) of the member(s) this attribute belongs to

#### Members Uri
`VxUriAttribute` applies to input properties of type `Uri` and instruct the File-Open dialog to display the right files and browse the correct data pool (e.g. *Textures* or *Data*)
* **DataPools** List of valid data pools
* **Extensions** List of file extensions

#### Enum Flags
`VxFlagGroupNameAttribute` and `VxFlagGroupItemAttribute` are used to annotate custom enumerations and make these comfortable to use. Enumeration can container groups of fields. This groups may behave differently
* a group is a selection (or a sub-enumeration), or
* a group is a flags enumeration

Ventuz can combine both technologies in a single enumeration (.Net has the `[Flags]` attribute). This helps to put as much information as possible into a single enumeration.

A group is defined by a mask. This mask is the unique identifier of a group.

`VxFlagGroupNameAttribute` defines the display name of a group.
* **Mask** the bit-mask and identifier of the group
* **DisplayName** the display name of this group

`VxFlagGroupItemAttribute` must be set to ***every enum field***
* **Mask** the bit-mask and identifier of the group
* **IsEnum** indicates if this field is a flag (false) or a selection (true) within its group. It is recommended to have `IsEnum` identical to all fields within a group.
* **DisplayName** the display name of this field

Example:
```c#
[VxFlagGroupName(0x0f, "Select one or more flags")]
[VxFlagGroupName(0x30, "Select a color")]
[Flags]
public enum MyFlags
{
    // flags: IsEnum=false
    [VxFlagGroupItem(0x0f, false, DisplayName = "For A")]
    A = 1,
    [VxFlagGroupItem(0x0f, false, DisplayName = "For B")]
    B = 2,
    [VxFlagGroupItem(0x0f, false, DisplayName = "For C")]
    C = 4,
    [VxFlagGroupItem(0x0f, false, DisplayName = "For D")]
    D = 8,

    // selection: IsEnum=true
    [VxFlagGroupItem(0x30, true, DisplayName = "None")]
    N = 0x00,
    [VxFlagGroupItem(0x30, true, DisplayName = "Red")]
    R = 0x10,
    [VxFlagGroupItem(0x30, true, DisplayName = "Yellow")]
    Y = 0x20,
    [VxFlagGroupItem(0x30, true, DisplayName = "Green")]
    G = 0x30,
}
```

***If any flags are used (IsEnum=false) it is recommended to also add the `[Flags]` attribute.***

## Roadmap

Ventuz Extensions will be continuously developed to provide new possibilities and improvements. Here is an ___unsorted___ list of planned changes and ideas:

* Deployment via NuGet Technology, Vx user interface to install/uninstall vx assemblies
* Code Security and Digital Signatures for trusted code
* Licensing for 3rd Party VX-Code
* Use .net 6 for re- and unloading Vx-Code as well for faster and saver access to native memory
* Use of .net 6 `Span<T>` class to access native memory
* API: Vx...Container types (Content, Hierarchy and Layer)
* Generate custom Material resources
* more mesh types: TriangleFan, TriangleList, PointList, LineList, QuadList, LineListAdjacency
* ref parameters for intermediate properties (in/out to use the backing store)
* `CreateTextRenderObject3D` to create 3D text resources to be rendered
* `Method...` function should also be able to receive properties from backing store
* localization of metadata
* give more access to file IO via UriManager plus some Uri helper function to deal with Ventuz Uris
* control Animations in validation phase
* define content nodes as "binding transformer" to be dropped on bindings in Content Editor
* async resource generator for long-tasks
* improve performance of mesh instance rendering
* high-performance texture updates (dynamic upload, double buffering, async)
* give access to the full tree, all nodes for create, find, delete, modify
* create modify animations
* API for touch interactions
* API for modifying IText to create custom text-effects
* custom IPP shader (layer) effects
* API to "custom model" (dynamic properties created by user)
* Data Portals (also as internal nodes) to send and receive data packets without binding
* define and use property groups
* access to more UI settings (like design color of nodes, annotations, positioning of content nodes, content families, ...)
* notification to the vx-code if scene was modified by designer and/or code

