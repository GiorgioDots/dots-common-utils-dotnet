import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
import { notAuthGuard } from './guards/not-auth.guard';
import { LoginComponent } from './pages/login/login.component';
import { HomeComponent } from './pages/home/home.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent, canActivate: [notAuthGuard] },
  { path: '', component: HomeComponent, canActivate: [authGuard] },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
