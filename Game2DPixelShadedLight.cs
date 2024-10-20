using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonogamePublicExample;

public class Game2DPixelShadedLight : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    
    Point resolutionWindow;

    Effect FXLightShader;
    Effect FXMaskShader;

    RenderTarget2D RT2World;
    RenderTarget2D RT2DdarkMask;
    RenderTarget2D RT2DLightMask;
    RenderTarget2D RT2DWorldPlusLightMask;

    Texture2D T2DLightMask;
    Texture2D T2dCowboy;
    Texture2D T2DWorld;

    
    float lightIntensity = 0.5f;
    float shadeIntensity = 0.6f;
    Vector2 playerPos = new Vector2(100, 100);
    float playerShadeRadius = 120;


    public Game2DPixelShadedLight()
    {
        _graphics = new GraphicsDeviceManager(this);
        base.Content.RootDirectory = "Content";
        IsMouseVisible = true;

        var screenSize = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;

        //<TODO> remove this minus later, it was for testing
        resolutionWindow = new Point( screenSize.Width , screenSize.Height );

    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);


        // TODO: use this.Content to load your game content here
        RT2World = new RenderTarget2D(GraphicsDevice, resolutionWindow.X, resolutionWindow.Y);
        RT2DdarkMask = new RenderTarget2D(GraphicsDevice, resolutionWindow.X, resolutionWindow.Y);
        RT2DLightMask = new RenderTarget2D(GraphicsDevice, resolutionWindow.X, resolutionWindow.Y);
        RT2DWorldPlusLightMask = new RenderTarget2D(GraphicsDevice, resolutionWindow.X, resolutionWindow.Y);

        FXLightShader = Content.Load<Effect>("FX/LightShader");
        FXMaskShader = Content.Load<Effect>("FX/MaskShader");

        T2DLightMask = Content.Load<Texture2D>("png/lightMaps");
        T2dCowboy = Content.Load<Texture2D>("png/cowboy");
        T2DWorld = Content.Load<Texture2D>("png/testmap");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();


        playerPos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);



        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);





        //light to rendertarget = RT2DLightMask
        GraphicsDevice.SetRenderTarget(RT2DLightMask);
        GraphicsDevice.Clear(Color.Transparent);
        _spriteBatch.Begin();


        //!lights from the light mask Texture2D
        Rectangle bigLight = new Rectangle(48, 0, 48, 48);
        Vector2 bigLightPos = new Vector2(386,14);
        Vector2 bigLightPos2 = new Vector2(360,-12);
        Vector2 bigLightPos3 = new Vector2(680,-36);
        Vector2 bigLightPos4 = new Vector2(336,84);
        Vector2 bigLightPos5 = new Vector2(680,200);

        _spriteBatch.Draw(T2DLightMask, bigLightPos, bigLight, Color.Yellow * 0.6f, 0, Vector2.Zero, 3f, SpriteEffects.None, 0.001f);
        _spriteBatch.Draw(T2DLightMask, bigLightPos4, bigLight, Color.Yellow * 0.5f , 0, Vector2.Zero, 1f, SpriteEffects.None, 0.001f);
        _spriteBatch.Draw(T2DLightMask, bigLightPos2, bigLight, Color.Red , 0, Vector2.Zero, 2f, SpriteEffects.None, 0.001f);
        _spriteBatch.Draw(T2DLightMask, bigLightPos3, bigLight, Color.Blue * 0.6f, 0, Vector2.Zero, 4f, SpriteEffects.None, 0.001f);
        _spriteBatch.Draw(T2DLightMask, bigLightPos5, bigLight, Color.Blue * 0.6f, 0, Vector2.Zero, 4f, SpriteEffects.None, 0.001f);



        _spriteBatch.End();


        //dark mask screen to render target = RT2DdarkMask
        //!everything that is not lit will get a dark gradient or complete black
        FXMaskShader.Parameters["playerShadeRadius"].SetValue(playerShadeRadius);
        FXMaskShader.Parameters["playerPos"].SetValue(playerPos);
        _spriteBatch.GraphicsDevice.SetRenderTarget(RT2DdarkMask );
        GraphicsDevice.Clear(Color.Transparent);
        _spriteBatch.Begin(
            SpriteSortMode.BackToFront,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.Default,
            effect: FXMaskShader
        );
        _spriteBatch.Draw(RT2DLightMask, Vector2.Zero, RT2DLightMask.Bounds, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.001f);
        _spriteBatch.End();



        //transparent with some gradient ligths on it
        GraphicsDevice.SetRenderTarget(RT2World);
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin(
            SpriteSortMode.BackToFront,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.Default
        );
        _spriteBatch.Draw(T2DWorld, Vector2.Zero, T2DWorld.Bounds, Color.White, 0, Vector2.Zero, 3f, SpriteEffects.None, 0.009f);
        //! draw other stuff here, character props, walls etc
        //! draw other stuff here, character props, walls etc
        //! draw other stuff here, character props, walls etc
        Rectangle rectcowboy = new Rectangle(0, 0, 64, 64);
        _spriteBatch.Draw(T2dCowboy, playerPos - new Vector2(32,32), rectcowboy, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.002f);

        _spriteBatch.End();


        //!combine world and light mask
        //!normally you would want to take pixels from the world and apply bloom to the brightest colors in your world, to make it pop
        //!i have not done that here
        FXLightShader.Parameters["lightIntensity"].SetValue(lightIntensity);
        FXLightShader.Parameters["MaskTextureLights"].SetValue(RT2DLightMask);
        // FXLightShader.Parameters["RenderTargetTexture"].SetValue(RT2World);
        _spriteBatch.GraphicsDevice.SetRenderTarget(RT2DWorldPlusLightMask );
        GraphicsDevice.Clear(Color.Transparent);
        _spriteBatch.Begin(
            SpriteSortMode.BackToFront,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.Default,
            effect: FXLightShader
        );
        _spriteBatch.Draw(RT2World, Vector2.Zero, RT2World.Bounds, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.001f);
        _spriteBatch.End();



        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin();

 
        // _spriteBatch.Draw(RT2DLightMask, Vector2.Zero, RT2DWorldPlusLightMask.Bounds, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.003f);
        _spriteBatch.Draw(RT2DWorldPlusLightMask, Vector2.Zero, RT2DWorldPlusLightMask.Bounds, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.003f);
        _spriteBatch.Draw(RT2DdarkMask, Vector2.Zero, RT2DdarkMask.Bounds, Color.White * shadeIntensity, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.002f);

        _spriteBatch.End();



        base.Draw(gameTime);
    }

    protected override void UnloadContent()
    {
        FXLightShader.Dispose();
    }






}
