import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth/auth.service';

export const notAuthGuard: CanActivateFn = (route, state) => {
  const authSvc = inject(AuthService);
  if (authSvc.isAuth) {
    const router = inject(Router);
    router.navigate(['']);
    return false;
  }
  return true;
};
