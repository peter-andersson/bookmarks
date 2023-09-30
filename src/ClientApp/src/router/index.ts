import { createRouter, createWebHistory } from 'vue-router'
import BookmarksView from '../views/BookmarksView.vue'
import AddView from '../views/AddView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'bookmarks',
      component: BookmarksView
    },
    {
      path: '/add',
      name: 'add',
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: AddView
    }
  ]
})

export default router
