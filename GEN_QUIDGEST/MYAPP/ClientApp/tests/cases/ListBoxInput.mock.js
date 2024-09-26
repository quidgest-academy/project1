export default {
	simpleUsage()
	{
		return {
			id: 'test',
			size: '235',
			label: 'Best Web Development Framework',
			//default selected value
			listVar: 2,
			listSmallVar: 2,
			listMediumVar: '2',
			listLargeVar: 2,
			listUnselectedVar: '',
			defaultNoOfItems: '1',
			//Sample list items
			simpleListItems: [
				{
					Value: 1,
					Text: 'Rails',
					disabled: true,
					Image: 'https://upload.wikimedia.org/wikipedia/commons/9/9c/Ruby_on_Rails_logo.jpg',
					Title: 'Rails Title',
					Description: 'An approachable, performant and versatile framework for building web user interfaces.',
					alt: 'Rails logo'
				},
				{
					Value: 2,
					Text: 'Django',
					disabled: false,
					Image: 'https://velog.velcdn.com/images/markyang92/post/920d793f-e764-4729-b3b8-b55fec534fc0/img59.png',
					Title: 'Django Title',
					Description: 'Django Description',
					alt: 'Django logo'
				},
				{
					Value: 3,
					Text: 'Angular',
					Image: 'https://angular.io/assets/images/logos/angular/angular.svg',
					Title: 'Angular Title',
					Description: 'Angular Description',
					alt: 'Angular logo'
				},
				{
					Value: 4,
					Text: 'Vue.js',
					Image: 'https://upload.wikimedia.org/wikipedia/commons/thumb/9/95/Vue.js_Logo_2.svg/300px-Vue.js_Logo_2.svg.png',
					Title: 'Vue.js Title',
					Description: 'Vue.js Description',
					alt: 'Vue.js logo'
				},
				{
					Value: 5,
					Text: 'React',
					Image: 'https://upload.wikimedia.org/wikipedia/commons/thumb/a/a7/React-icon.svg/768px-React-icon.svg.png',
					Title: 'React Title',
					Description: 'React Description',
					alt: 'React logo'
				},
				{
					Value: 6,
					Text: 'Python',
					Image: 'https://upload.wikimedia.org/wikipedia/commons/thumb/c/c3/Python-logo-notext.svg/172px-Python-logo-notext.svg.png',
					Title: 'Python Title',
					Description: 'Python Description',
					alt: 'Python logo'
				},
				{
					Value: 7,
					Text: 'ASP.net',
					Image: 'https://icons-for-free.com/iconfiles/png/512/asp-1320184805417750774.png',
					Title: 'ASP.net Title',
					Description: 'ASP.net Description',
					alt: 'ASP.net logo'
				}
			],
			//Sample list items with lengthy values
			simpleList: [
				{
					Value: 1,
					Text: 'Ruby on Rails, or Rails, is a server-side web application framework written in Ruby under the MIT License. Rails is a model–view–controller framework, providing default structures for a database, a web service, and web pages',
					Image: 'https://upload.wikimedia.org/wikipedia/commons/9/9c/Ruby_on_Rails_logo.jpg',
					Title: 'Ruby on Rails Long',
					Description: 'Long description for Ruby on Rails',
					alt: 'Ruby logo'
				},
				{
					Value: 2,
					Text: 'Django is a Python-based free and open-source web framework that follows the model–template–views architectural pattern.',
					Image: 'https://velog.velcdn.com/images/markyang92/post/920d793f-e764-4729-b3b8-b55fec534fc0/img59.png',
					Title: 'Django Long',
					Description: 'Long description for Django',
					alt: 'Django logo'
				},
				{
					Value: 3,
					Text: 'Angular',
					Image: 'https://angular.io/assets/images/logos/angular/angular.svg',
					Title: 'Angular Long',
					Description: 'Long description for Angular',
					alt: 'Angular logo'
				},
				{
					Value: 4,
					Text: 'Vue.js',
					Image: 'https://upload.wikimedia.org/wikipedia/commons/thumb/9/95/Vue.js_Logo_2.svg/300px-Vue.js_Logo_2.svg.png',
					Title: 'Vue.js Long',
					Description: 'Long description for Vue.js',
					alt: 'Vue.js logo'
				},
				{
					Value: 5,
					Text: 'React',
					Image: 'https://upload.wikimedia.org/wikipedia/commons/thumb/a/a7/React-icon.svg/768px-React-icon.svg.png',
					Title: 'React Long',
					Description: 'Long description for React',
					alt: 'React logo'
				},
				{
					Value: 6,
					Text: 'Python',
					Image: 'https://upload.wikimedia.org/wikipedia/commons/thumb/c/c3/Python-logo-notext.svg/172px-Python-logo-notext.svg.png',
					Title: 'Python Long',
					Description: 'Long description for Python',
					alt: 'Python logo'
				},
				{
					Value: 7,
					Text: 'ASP.net',
					Image: 'https://neosmart.net/blog/wp-content/uploads/2019/06/dot-NET-Core-300x300.png',
					Title: 'ASP.net Long',
					Description: 'Long description for ASP.net',
					alt: 'ASP.net logo'
				}
			],
			listVarData: 2,
			NumberOfItemsToShowOne: 0,
			NumberOfItemsToShowTwo: 6,
			readonly: true,
			noOfItemsForSizeSmall: 4,
			noOfItemsForSizeMedium: 4,
			noOfItemsForSizeLarge: 3,
			noOfItemsForSizeXl: 4,
			simpleListItemsForSize: [
				{
					Value: 1,
					Text: 'Rails',
					Image: 'https://upload.wikimedia.org/wikipedia/commons/9/9c/Ruby_on_Rails_logo.jpg',
					Title: 'Ruby on Rails',
					Description: 'Rails short description',
					alt: 'Rails logo'
				},
				{
					Value: 2,
					Text: 'Django',
					Image: 'https://velog.velcdn.com/images/markyang92/post/920d793f-e764-4729-b3b8-b55fec534fc0/img59.png',
					Title: 'Django',
					Description: 'Short description for Django',
					alt: 'Django logo'
				},
				{
					Value: 3,
					Text: 'Angular',
					Image: 'https://angular.io/assets/images/logos/angular/angular.svg',
					Title: 'Angular',
					Description: 'Short description for Angular Size',
					alt: 'Angular logo'
				},
				{
					Value: 4,
					Text: 'Vue.js',
					Image: 'https://upload.wikimedia.org/wikipedia/commons/thumb/9/95/Vue.js_Logo_2.svg/300px-Vue.js_Logo_2.svg.png',
					Title: 'Vue.js',
					Description: 'Vue.js short description',
					alt: 'Vue.js logo'
				}
			]
		}
	}
}
