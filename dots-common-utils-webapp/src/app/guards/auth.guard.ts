import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const authSvc = inject(AuthService);

  if (authSvc.isAuth) {
    return true;
  }
  
  router.navigate(['login']);
  return false;
};
