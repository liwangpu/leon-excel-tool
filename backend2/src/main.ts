import { NestFactory } from '@nestjs/core';
import { AppModule } from './app.module';
import { WsAdapter } from './ws.adapter';

async function bootstrap() {
    const app = await NestFactory.create(AppModule, { cors: true });
    // app.useWebSocketAdapter(new WsAdapter(app)); // 使用我们的适配器
    await app.listen(3000);
}
bootstrap();
