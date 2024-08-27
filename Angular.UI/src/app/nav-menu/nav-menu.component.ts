import { Component } from '@angular/core';

@Component({
  selector: 'app-nav-menu',
  standalone: true,
  imports: [],
  templateUrl: './nav-menu.component.html',
  styleUrl: './nav-menu.component.css'
})
export class NavMenuComponent {
  navbarOpened: boolean = false;
  languageMenuOpened: boolean = false;

  ToogleNavbar() {    
    this.navbarOpened = !this.navbarOpened;
  }

  ExpandSubMenu(event:any) {
    var dropdownmenu = document.getElementById(event.target.id + "-child") as HTMLElement;
    dropdownmenu.style.display = dropdownmenu.style.display == 'none' ? 'block' : 'none';
  }

  ToogleLanguageMenu() {
    this.languageMenuOpened = !this.languageMenuOpened;
  }

}
