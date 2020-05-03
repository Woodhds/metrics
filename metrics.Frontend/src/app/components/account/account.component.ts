import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormControl, FormGroup} from '@angular/forms';
import {AppConfig} from '../../models/AppConfig';
import {AppConfigService} from '../../services/concrete/AppConfig/AppConfigService';
import {IAuthService} from '../../services/abstract/IAuth';
import {ActivatedRoute, Router} from '@angular/router';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.scss']
})
export class AccountComponent implements OnInit {

  form: FormGroup;
  public config: AppConfig;

  constructor(
    private fb: FormBuilder,
    private appConfigService: AppConfigService,
    private authService: IAuthService,
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) {
    this.form = fb.group({
      token: new FormControl('')
    });
  }

  ngOnInit() {
    this.appConfigService.getConfig().subscribe(f => {
      this.config = f;
    });
  }

  get href() {
    return this.config ? 'https://oauth.vk.com/authorize?client_id=' + this.config.ClientId + `&display=page&redirect_uri=http://vk.com/blank.php&scope=111111111&response_type=token&v=5.85` : '';
  }

  onSubmit(): void {
    this.authService.login(this.form.value).subscribe(() => {
      const returnUrl = this.activatedRoute.snapshot.params.returnUrl;
      if (returnUrl) {
        this.router.navigate([returnUrl])
      }
    })
  }
}
