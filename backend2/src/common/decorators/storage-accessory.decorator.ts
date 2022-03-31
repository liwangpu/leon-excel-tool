import { createParamDecorator, ExecutionContext } from '@nestjs/common';

export const StorageAccessory = createParamDecorator((data: unknown, ctx: ExecutionContext) => {
    const request = ctx.switchToHttp().getRequest();
    // return request.user;
    console.log('re:', request);
    return {};
});